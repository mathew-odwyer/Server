/// @description Represents a network server.
/// @param {Real} port The port number.
/// @param {Real} max_clients The maximum number of client connections.
function Server(port, max_clients) constructor
{
	/// @type {Struct.Logger}
	/// @description The logger.
	_logger = new Logger(nameof(Server));
	
	/// @type {Id.Socket}
	/// @description The underlying server socket.
	_server = network_create_server(network_socket_tcp, port, max_clients);

	/// @type {Id.DsMap<Real, Struct.ClientConnection>}
	/// @description The socket identifier to client connection map. 
	_socket_to_connection_map = ds_map_create();
	
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
		var connection = new ClientConnection(socket);
		ds_map_add(_socket_to_connection_map, socket, connection);
		
		_logger.log(log_type.debug, $"Socket connected with ID: '{socket}'!");
		
		return connection;
	}
	
	/// @description Removes a client connection from the server.
	/// @param {Id.Socket} socket The socket identifier used to remove the client connection.
	remove_connection = function(socket)
	{
		var connection = _socket_to_connection_map[? socket];
		
		if (!is_undefined(connection))
		{
			connection.disconnect();
			ds_map_delete(_socket_to_connection_map, socket);
			
			_logger.log(log_type.debug, $"Socket disconnected with ID: '{socket}'");
		}
	}
	
	/// @description Gets the client connection that matches the specified `socket`.
	/// @param {Id.Socket} socket The socket identifier used to retrieve the client connection.
	/// @returns {Struct.ClientConnection} Returns the client connection associated with the `socket`.
	get_connection = function(socket)
	{
		return _socket_to_connection_map[? socket];
	}
	
	/// @description Broadcasta a `message` to all registered connections.
	/// @param {String} message The message to send to all connected clients.
	broadcast = function(message)
	{
		var connections = ds_map_values_to_array(_socket_to_connection_map);
		
		_logger.log(log_type.trace, $"Broadcasting message: '{message}'");
		
		for (var i = 0; i < array_length(connections); i++)
		{
			connections[i].send(message);
		}
	}

	/// @description Cleanup resources.
	cleanup = function()
	{
		var connections = ds_map_values_to_array(_socket_to_connection_map);
		
		array_foreach(connections, function(connection)
		{
			connection.disconnect();
		});
		
		ds_map_destroy(_socket_to_connection_map);
		network_destroy(_server);
	}
}
