/// @description Represents a network server.
/// @param {Constant.SocketType} type The socket type.
/// @param {Real} port The port number.
/// @param {Real} max_clients The maximum number of client connections.
function Server(type, port, max_clients) constructor
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(Server));
	
	/// @type {Id.Socket}
	/// @description The underlying server socket.
	_server = network_create_server_raw(type, port, max_clients);

	/// @type {Struct}
	/// @description The socket identifier to client connection map. 
	_socket_to_connection_map = {};
	
	if (_server < 0)
	{
		throw "Failed to create network server socket.";
	}
	
	_logger.log(log_type.information, $"Server listening on port: '{port}'");
	
	/// @description Adds a new client connection to the server.
	/// @param {Id.Socket} socket The socket identifier used to create the client connection.
	/// @returns {Struct.ClientConnection} Returns the new client connection.
	add_connection = function(socket)
	{
		var connection = new ClientConnection(socket);
		_socket_to_connection_map[$ string(socket)] = connection;
		
		_logger.log(log_type.information, $"Socket connected with ID: '{socket}'!");
		
		return connection;
	}
	
	/// @description Removes a client connection from the server.
	/// @param {Id.Socket} socket The socket identifier used to remove the client connection.
	remove_connection = function(socket)
	{
		var connection = _socket_to_connection_map[$ string(socket)];
		
		if (!is_undefined(connection))
		{
			connection.cleanup();
			struct_remove(_socket_to_connection_map, string(socket));
			
			_logger.log(log_type.information, $"Socket disconnected with ID: '{socket}'");
		}
	}
	
	/// @description Gets the client connection that matches the specified `socket`.
	/// @param {Id.Socket} socket The socket identifier used to retrieve the client connection.
	/// @returns {Struct.ClientConnection} Returns the client connection associated with the `socket`.
	get_connection = function(socket)
	{
		return _socket_to_connection_map[$ string(socket)];
	}

	/// @description Cleanup resources.
	cleanup = function()
	{
		network_destroy(_server);
	}
}
