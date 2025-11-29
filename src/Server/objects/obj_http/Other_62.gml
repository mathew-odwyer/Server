/// @description Handle HTTP events.

var request = async_load[? "id"];
var result = async_load[? "result"];
	
var status = async_load[? "status"];
var http_status = async_load[? "http_status"];

if (status == 1)
{
	// Still downloading...
	exit;
}

var context = http_async.RequestToContextMap[$ request];

if (is_undefined(context))
{
	_logger.log(log_type.warning, $"Failed to locate HTTP context for request: '{request}'");
	return;
}

var data = {};

var resolve = context.resolve;
var reject = context.reject;
var timeout = context.timeout;

var options = context.options;
var signal = options[$ "signal"];

if (!string_empty(result))
{
	try
	{
		data = json_parse(result);
	}
	catch (ex)
	{
		throw $"Failed to parse incoming response for result: '{result}'";
	}
}

if (!is_undefined(signal) && signal.get_aborted())
{
	// If the operation ahs been cancelled, abort.
	_logger.log(log_type.debug, $"HTTP Asynchronous operation has been aborted: {signal.get_reason()}");
	reject(signal.get_reason());
}
else if (status == 0 && http_status >= 200 && http_status <= 299)
{
	// Success
	resolve(data);
}
else
{
	// Convert to JSON-RPC error response.
	if (options[$ "jsonrpc_error"] ?? false)
	{
		data = _convert_problem_details_to_jsonrpc(http_status, data);
	}
	
	// Failure
	reject(data);
}

call_cancel(timeout);

/// @feather ignore once GM1041
struct_remove(http_async.RequestToContextMap, request);
