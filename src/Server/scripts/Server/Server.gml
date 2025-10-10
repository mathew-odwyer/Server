/// @description Represents a network server with JSON-RPC 2.0 protocol handling.
/// @param {Constant.SocketType} type The network socket type (tcp, wss, etc).
/// @param {Real} port The port number.
/// @param {Real} max_clients The maximum number of client connections.
function Server(type, port, max_clients) constructor
{
	/// @type {Id.Socket}
	/// @description The underlying server socket.
	_server = network_create_server(type, port, max_clients);

	/// @type {Id.DsMap<Real, Struct.ClientConnection>}
	/// @description The socket identifier to client connection map. 
	_socket_to_connection_map = ds_map_create();

	/// @type {Id.DsMap<String|Real, Function}
	/// @description The client request to handler map.
	_request_to_handler_map = ds_map_create();
	
	if (_server < 0)
	{
		throw "Failed to create network server socket.";
	}
	
	/// @description Registers a JSON-RPC 2.0 notification handler.
	/// @param {String} method The method/procedure name to register.
	/// @param {Function} callback The callback to execute when the notification arrives.
    register_handler = function(method, callback)
    {
		show_debug_message($"Registering RPC Handler: '{method}'");
        _request_to_handler_map[? method] = callback;
    }
	
	/// @description Adds a new client connection to the server.
	/// @param {Real} socket The socket identifier used to create the client connection.
	/// @returns {Struct.ClientConnection} Returns the new client connection.
	add_connection = function(socket)
	{
		var connection = new ClientConnection(socket);
		ds_map_add(_socket_to_connection_map, socket, connection);
		
		return connection;
	}
	
	/// @description Removes a client connection from the server.
	/// @param {Real} socket The socket identifier used to remove the client connection.
	remove_connection = function(socket)
	{
		var connection = _socket_to_connection_map[? socket];
		
		if (!is_undefined(connection))
		{
			connection.disconnect();
			ds_map_delete(_socket_to_connection_map, socket);
		}
	}
	
	/// @description Sends a JSON-RPC 2.0 notification to the specified `connection`.
	/// @param {Struct.ClientConnection} connection The client connection to receive the notification.
	/// @param {String} method The name of the method to invoke on the client.
	/// @param {Struct|Undefined} params The method parameters to send to the client (if any).
	notify = function(connection, method, params = undefined)
	{
		connection.notify(method, params);
	}
	
	/// @description Broadcasta a JSON-RPC 2.0 notification to all registered connections.
	/// @param {String} method The name of the method to invoke on the client.
	/// @param {Struct|Undefined} params The method parameters to send to the client (if any).
	broadcast = function(method, params = undefined)
	{
		var connections = ds_map_values_to_array(_socket_to_connection_map);
		
		for (var i = 0; i < array_length(connections); i++)
		{
			connections[i].notify(method, params);
		}
	}
	
	/// @description Handles an incoming request or notification from a socket connection.
	/// @param {Real} socket The socket identifier.
	/// @param {String} payload The payload to parse.
	handle_message = function(socket, payload)
	{
		var connection = _socket_to_connection_map[? socket];
		var message = undefined;
		
		// Unknown socket, just ignore the message.
		if (is_undefined(connection))
		{
			return;
		}
		
		try
		{
			message = json_parse(payload);
		}
		catch (ex)
		{
			connection.send_error(undefined, -32700, "Parse error");
			return;
		}
		
		var jsonrpc = struct_get(message, "jsonrpc");
        var rpc_id = struct_get(message, "id");
		
		var procedure = struct_get(message, "method");
        var params = struct_get(message, "params");
		
		var is_request = !is_undefined(rpc_id);
		
		if (is_undefined(jsonrpc) || jsonrpc != "2.0" ||
			is_undefined(procedure) || !is_string(procedure))
        {
			connection.send_error(rpc_id, -32600, "Invalid Request");
			return;
        }
		
		var handler = _request_to_handler_map[? procedure];

		if (is_undefined(handler))
		{
			if (is_request)
			{
				connection.send_error(rpc_id, -32601, "Method Not Found");
			}
			
			return;
		}
		
		try
		{
			var result = method(self, handler)(params, connection, rpc_id);
			
			if (is_request && !is_undefined(result))
			{
				connection.send_result(rpc_id, result);
			}
		}
		catch (ex)
		{
			if (is_request)
			{
				if (!is_instanceof(ex, RpcException))
				{
					show_debug_message($"Internal server error: {ex}");
					connection.send_error(rpc_id, -32603, "Internal Error");
					return;
				}
				
				connection.send_error(rpc_id, -32000, instanceof(ex), ex);
			}
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
		
		ds_map_destroy(_request_to_handler_map);
		ds_map_destroy(_socket_to_connection_map);
		network_destroy(_server);
	}
}