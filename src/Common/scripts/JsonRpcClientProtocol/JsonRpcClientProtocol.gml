function JsonRpcClientProtocol(send) constructor
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(JsonRpcClientProtocol));
	
	/// @type {Function}
	/// @description The function used to send messages.
	_send = send;
	
	/// @type {Id.DsMap}
	/// @description The command/request to handler map.
	_command_to_handler_map = {};
	
	/// @description Registers a JSON-RPC 2.0 notification handler.
	/// @param {String} procedure The procedure to register.
	/// @param {Function} callback The callback to execute when the notification arrives.
	register = function(procedure, callback)
	{
		_logger.log(log_type.information, $"Registering notification handler for: '{procedure}'...");
		_command_to_handler_map[$ procedure] = callback;
	}
	
	/// @description Sends a JSON-RPC 2.0 notification to the server.
	/// @param {String} procedure The procedure to send to the server.
	/// @param {Struct} params To the parameteres of the procedure to send to the server.
	notify = function(procedure, params = {})
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
	/// @param {Real} timeout_delay The number of seconds to pass before rejecting the `Struct.Promise`.
	/// @returns {Struct.Promise} Returns the `Promise` associated with the call.
	call = function(procedure, params = {}, timeout_delay = 5)
	{
		static rpc_id = 0;
		rpc_id++;
		
		var promise = new Promise();
		var timeout = set_timeout(method({promise}, function() {
			promise.reject(new Error("The request timed out."));
		}), timeout_delay);
		
		promise
			.next(method({timeout}, function(result)
			{
				clear_timeout(timeout);
				return result;
			}))
			.fail(method({timeout}, function(error)
			{
				clear_timeout(timeout);
				throw error;
			}));
		
		var payload = {
			jsonrpc: "2.0",
			id: rpc_id,
			method: procedure,
			params: params,
		};
		
		_command_to_handler_map[$ rpc_id] = promise;
		_send(payload);
		
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
			_logger.log(log_type.error, "Received malformed JSON from server.");
			return;
		}
		
		var jsonrpc = struct_get(message, "jsonrpc");
		var rpc_id = struct_get(message, "id");
		
		if (is_undefined(jsonrpc) || jsonrpc != "2.0")
        {
            _logger.log(log_type.error, "Received message with invalid/missing jsonrpc field; ignoring.");
            return;
        }
		
		if (!is_undefined(rpc_id))
		{
			var promise = _command_to_handler_map[$ rpc_id];
			
			if (is_undefined(promise))
			{
				_logger.log(log_type.error, $"Failed to locate request handler for ID: '{rpc_id}'");
				return;
			}
			
			struct_remove(_command_to_handler_map, rpc_id);
			
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
				var handler = _command_to_handler_map[$ procedure];
			
				if (is_undefined(handler))
				{
					_logger.log(log_type.error, $"Failed to locate handler for procedure: '{procedure}'");
					return;
				}
				
				handler(params);
			}	
		}
	}
}