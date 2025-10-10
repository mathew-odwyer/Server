/// @description Represents a network client with JSON-RPC 2.0 protocol handling.
/// @param {Constant.SocketType} type The network socket type (tcp, wss, etc).
function Client(type) constructor
{
	/// @type {Id.Socket}
	/// @description The underlying network socket.
	_socket = network_create_socket(type);

	/// @type {Id.Buffer}
	/// @description The buffer used to prepare requests.
	_write_buffer = buffer_create(1, buffer_grow, 1);

	/// @type {Id.DsMap<String|Real, Function|Struct.JsonRpcRequest>}
	/// @description Represents the request to handler map.
	_request_to_handler_map = ds_map_create();
	
	if (_socket < 0)
	{
		throw "Failed to create network socket.";
	}
	
	/// @description Connects the client to the specified `host` and `port`.
	/// @param {String} host The host name or URL.
	/// @param {Real} port The port number.
	/// @returns {Bool} Returns `true` if the client connected; otherwise, `false`.
	connect = function(host, port)
	{
		return network_connect_raw(_socket, host, port) >= 0;
	}
	
	/// @description Registers a JSON-RPC 2.0 notification handler.
	/// @param {String} method The method/procedure name to register.
	/// @param {Function} callback The callback to execute when the notification arrives.
    register_handler = function(method, callback)
    {
        _request_to_handler_map[? method] = callback;
    }
	
	/// @description Sends a JSON-RPC 2.0 notification to the server.
	/// @param {String} method The name of the method/procedure to invoke on the server.
	/// @param {Any} params The parameters of the method/procedure to send to the server.
	notify = function(method, params)
	{
		var payload = {
            jsonrpc: "2.0",
            method: method,
            params: params,
        };
		
		buffer_seek(_write_buffer, buffer_seek_start, 0);
        buffer_write(_write_buffer, buffer_text, json_stringify(payload));
		network_send_raw(_socket, _write_buffer, buffer_tell(_write_buffer));
	}
	
	/// @description Sends a JSON-RPC 2.0 request to the server.
	/// @param {String} method The name of the request/method to invoke on the server.
	/// @param {Any} params The parameters of the requset/method to send to the server.
	/// @returns {Struct.JsonRpcRequest} Returns a `Struct.JsonRpcRequest` that is used to register callbacks.
	call = function(method, params)
	{
		static rpc_id = 0;
		rpc_id++;
		
		var payload = {
			jsonrpc: "2.0",
			id: rpc_id,
			method: method,
			params: params,
		};
		
		var request = new JsonRpcRequest();
		_request_to_handler_map[? rpc_id] = request;
		
		buffer_seek(_write_buffer, buffer_seek_start, 0);
        buffer_write(_write_buffer, buffer_text, json_stringify(payload));
		network_send_raw(_socket, _write_buffer, buffer_tell(_write_buffer));
		
		return request;
	}
	
	/// @description Handles an incoming notification or response from the server.
	/// @param {String} payload The payload to parse.
	handle_message = function(payload)
	{
		var message = undefined;
		
		try
		{
			message = json_parse(payload);
		}
		catch (ex)
		{
			show_debug_message("Error: Received malformed JSON from server.");
			return;
		}
		
		var jsonrpc = struct_get(message, "jsonrpc");
		var rpc_id = struct_get(message, "id");
		
		if (is_undefined(jsonrpc) || jsonrpc != "2.0")
        {
            show_debug_message("Error: Received message with invalid/missing jsonrpc field; ignoring.");
            return;
        }
		
		if (!is_undefined(rpc_id))
		{
			var request = _request_to_handler_map[? rpc_id];
			
			if (is_undefined(request))
			{
				show_debug_message($"Error: Failed to locate request handler for ID: '{rpc_id}'");
				return;
			}
			
			ds_map_delete(_request_to_handler_map, rpc_id);
			
			var result = struct_get(message, "result");
			var error = struct_get(message, "error");
			
			if (!is_undefined(result))
			{
				request.raise_success(result, self);
				return;
			}
			
			if (!is_undefined(error))
			{
				request.raise_error(error, self);
				return;
			}
			
			show_debug_message($"Error: Malformed response for ID: '{rpc_id}'");
			return;
		}
		
		var procedure = struct_get(message, "method");
        var params = struct_get(message, "params");
		
		if (!is_undefined(procedure))
		{
			var handler = _request_to_handler_map[? procedure];
			
			if (is_undefined(handler))
			{
				show_debug_message($"Error: Failed to locate handler for procedure: '{procedure}'");
				return;
			}
			
			handler(params, self);
		}
	}
	
	/// @description Cleanup resources.
	cleanup = function()
	{
		ds_map_destroy(_request_to_handler_map);
		buffer_delete(_write_buffer);
		network_destroy(_socket);
	}
}