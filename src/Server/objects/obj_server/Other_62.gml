
/// @description Handles HTTP requests.

var request = async_load[? "id"];
var result = async_load[? "result"]
var status = async_load[? "status"];
var http_status = async_load[? "http_status"];
var response_headers = async_load[? "response_headers"];

var context = http_async.RequestToContextMap[? request];
ds_map_delete(http_async.RequestToContextMap, request);

if (is_undefined(context))
{
	return;
}

var promise = context.promise;
var options = context.options;

var type = response_headers[? "Content-Type"];
var data = HttpBodyParser.Parse(type, result);

if (status == 0)
{
	if (http_status >= 200 && http_status <= 299)
	{		
		promise.resolve({
			status: http_status,
			data: data,
			options,
		});
	}
	else
	{
		promise.reject({
			status: http_status,
			data,
			options,
		});
	}
}
else if (status < 0)
{
	var real_status = http_status;
	
	if (http_status >= 200 && http_status <= 299)
	{
		// Don't be silly GM; how can there be a failure AND a 200 OK?
		// Assume 503 service unavailable if anything.
		real_status = 503;
	}
	
	promise.reject({
		status: real_status,
		data: data,
		options: options,
	});
}
