/// @description Represents a JSON-RPC 2.0 request callback mechanism.
function JsonRpcRequest() constructor
{
	/// @type {Function|Undefined}
	/// @description The callback to execute when the request succeeded.
	_success = undefined;

	/// @type {Function|Undefined}
	/// @description The callback to execute when the request failed.
	_error = undefined;
	
	/// @description Registers a callback to be executed when the request succeeds.
	/// @param {Function|Undefined} callback The callback to execute when the request succeeds.
	/// @returns {Struct.JsonRpcRequest} Returns `self` for method chaining.
	on_success = function(callback)
	{
		_success = callback;
		return self;
	}
	
	/// @description Registers a callback to be executed when the request fails.
	/// @param {Function|Undefined} callback The callback to execute when the request fails.
	/// @returns {Struct.JsonRpcRequest} Returns `self` for method chaining.
	on_error = function(callback)
	{
		_error = callback;
		return self;
	}
	
	/// @description Raises the success callback (if provided).
	/// @param {Any} result The result provided within the JSON-RPC 2.0 request.
	/// @param {Struct.Client} client The client that received the request.
	raise_success = function(result, client)
	{
		if (is_undefined(_success))
		{
			return;
		}
		
		_success(result, client);
	}
	
	/// @description Raises the error callback (if provided).
	/// @param {Any} error The error provided within the JSON-RPC 2.0 request.
	/// @param {Struct.Client} client The client that received the request.
	raise_error = function(error, client)
	{
		if (is_undefined(_error))
		{
			return;
		}
		
		_error(error, client);
	}
}