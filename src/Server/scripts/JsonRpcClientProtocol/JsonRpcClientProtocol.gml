function JsonRpcClientProtocol(send) constructor
{
	/// @type {Function}
	/// @description The function used to send messages.
	_send = send;
	
	/// @type {Id.DsMap}
	/// @description The command/request to handler map.
	_command_to_handler_map = ds_map_create();
	
	/// @description Registers a JSON-RPC 2.0 notification handler.
	/// @param {String} procedure The procedure to register.
	/// @param {Function} callback The callback to execute when the notification arrives.
	register = function(procedure, callback)
	{
		Logger.Log(log_type.information, $"Registering notification handler for: '{procedure}'...");
		_command_to_handler_map[? procedure] = callback;
	}
	
	/// @description Sends a JSON-RPC 2.0 notification to the server.
	/// @param {String} procedure The procedure to send to the server.
	/// @param {Struct} params To the parameteres of the procedure to send to the server.
	notify = function(procedure, params)
	{
		var payload = {
			jsonrpc: "2.0",
			method: procedure,
			params: params,
		};
	
		_send(payload);
	}
	
	/// @description Sends a JSON-RPC 2.0 request to the server.
	/// @param {String} procedure The procedure to send to the server.
	/// @param {Struct} params To the parameteres of the procedure to send to the server.
	/// @param {Real} timeout The number of seconds to pass before rejecting the `Promise`.
	/// @returns {Struct.Promise} Returns the `Promise` associated with the call.
	call = function(procedure, params, timeout = 5)
	{
		static rpc_id = 0;
		rpc_id++;
		
		var promise = new Promise()
			.timeout(timeout)
			.fail(method({_command_to_handler_map, rpc_id}, function(error)
			{	
				ds_map_delete(_command_to_handler_map, rpc_id);
				throw error;
			}));
		
		var payload = {
			jsonrpc: "2.0",
			id: rpc_id,
			method: procedure,
			params: params,
		};
		
		if (!_send(payload))
		{
			promise.reject(new SocketError($"Failed to send request: '{procedure}'"));
			return promise;
		}
		
		_command_to_handler_map[? rpc_id] = promise;
		
		return promise;
	}
	
	/// @description Handles an incoming `payload` from the server.
	/// @param {String} payload The payload sent to the client.
	handle_message = function(payload)
	{
		var message = undefined;
		
		try
		{
			message = json_parse(payload);
		}
		catch (ex)
		{
			Logger.Log(log_type.error, "Received malformed JSON from server.");
			return;
		}
		
		var jsonrpc = struct_get(message, "jsonrpc");
		var rpc_id = struct_get(message, "id");
		
		if (is_undefined(jsonrpc) || jsonrpc != "2.0")
        {
            Logger.Log(log_type.error, "Received message with invalid/missing jsonrpc field; ignoring.");
            return;
        }
		
		if (!is_undefined(rpc_id))
		{
			var promise = _command_to_handler_map[? rpc_id];
			
			if (is_undefined(promise))
			{
				Logger.Log(log_type.error, $"Failed to locate request handler for ID: '{rpc_id}'");
				return;
			}
			
			ds_map_delete(_command_to_handler_map, rpc_id);
			
			if (is_instanceof(promise, Promise))
			{
				var result = struct_get(message, "result");
				var error = struct_get(message, "error");
				
				if (!is_undefined(result))
				{
					promise.resolve(result);
				}
				else if (!is_undefined(error))
				{
					promise.reject(error);
				}
			}
		}
		else
		{
			var procedure = struct_get(message, "method");
	        var params = struct_get(message, "params");
		
			if (!is_undefined(procedure))
			{
				var handler = _command_to_handler_map[? procedure];
			
				if (is_undefined(handler))
				{
					Logger.Log(log_type.error, $"Failed to locate handler for procedure: '{procedure}'");
					return;
				}
				
				handler(params);
			}	
		}
	}
	
	/// @description Cleanup resources.
	cleanup = function()
	{
		ds_map_destroy(_command_to_handler_map);
	}
}