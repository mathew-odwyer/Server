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
	
	/// @type {Bool}
	/// @description Indicates whether this instance is disposed.
	_is_disposed = false;
	
	if (_server < 0)
	{
		throw new SocketError("Failed to create network server socket.");
	}
	
	_logger.log(log_type.information, $"Server listening on port: '{port}'");
	
	/// @description Adds a new client connection to the server.
	/// @param {Id.Socket} socket The socket identifier used to create the client connection.
	/// @returns {Struct.ClientConnection} Returns the new client connection.
	add_connection = function(socket)
	{
		if (_is_disposed)
		{
			throw new ObjectDisposedError(nameof(Server));
		}
		
		var connection = new ClientConnection(socket);
		_socket_to_connection_map[$ string(socket)] = connection;
		
		_logger.log(log_type.information, $"Socket connected with ID: '{socket}'!");
		
		return connection;
	}
	
	/// @description Removes a client connection from the server.
	/// @param {Id.Socket} socket The socket identifier used to remove the client connection.
	remove_connection = function(socket)
	{
		if (_is_disposed)
		{
			throw new ObjectDisposedError(nameof(ClientConnection));
		}
		
		var connection = _socket_to_connection_map[$ string(socket)];
		
		if (is_instanceof(connection, ClientConnection))
		{
			connection.cleanup();
			struct_remove(_socket_to_connection_map, string(socket));
			
			_logger.log(log_type.information, $"Socket disconnected with ID: '{socket}'");
		}
	}
	
	/// @description Removes all client connections from the server.
	remove_all_connections = function()
	{
		if (_is_disposed)
		{
			throw new ObjectDisposedError(nameof(ClientConnection));
		}
		
		_logger.log(log_type.information, "Removing all connections from the server...");
		
		var names = struct_get_names(_socket_to_connection_map);
		var length = array_length(names);
		
		for (var i = 0; i < length; i++)
		{
			_socket_to_connection_map[$ names[i]].cleanup();
		}
	}
	
	/// @description Gets the client connection that matches the specified `socket`.
	/// @param {Id.Socket} socket The socket identifier used to retrieve the client connection.
	/// @returns {Struct.ClientConnection} Returns the client connection associated with the `socket`.
	get_connection = function(socket)
	{
		if (_is_disposed)
		{
			throw new ObjectDisposedError(nameof(ClientConnection));
		}
		
		return _socket_to_connection_map[$ string(socket)];
	}

	/// @description Cleanup resources.
	cleanup = function()
	{
		if (_is_disposed)
		{
			return;
		}
		
		remove_all_connections();
		
		if (_server != -1)
		{
			network_destroy(_server);
			_server = -1;
		}
		
		_is_disposed = true;
	}
}
