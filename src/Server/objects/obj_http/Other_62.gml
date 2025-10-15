var request = async_load[? "id"];
var result = async_load[? "result"];
	
var status = async_load[? "status"];
var http_status = async_load[? "http_status"];

var context = http_async.RequestToContextMap[? request];
ds_map_delete(http_async.RequestToContextMap, request);

if (is_undefined(context))
{
	Logger.Log(log_type.warning, $"Failed to locate HTTP context for request: '{request}'");
	return;
}

var promise = context.promise;
var data = {};

if (is_string(result) && !string_empty(result))
{
	try
	{
		data = json_parse(result);
	}
	catch (ex)
	{
		throw new HttpError($"Failed to parse incoming response for result: '{result}'");
	}
}

if (status == 0 && http_status >= 200 && http_status <= 299)
{
	promise.resolve({
		status: http_status,
		data: data,
	});
}
else
{
	if (status < 0 && http_status >= 200 && http_status <= 299)
	{
		http_status = 500;
		
		data = {
			title: "Internal Server Error",
			detail: "An unexpected error occurred. Please try again later.",
			type: "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
			status: 500,
		};
		
		Logger.Log(log_type.warning, "Bad GML Status Code: Server likely unavailable.");
	}
		
	promise.reject({
		status: http_status,
		data: data,
	});
}