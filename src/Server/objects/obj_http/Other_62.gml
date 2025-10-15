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
var options = context.options;
var data = {};

try
{
	if (!is_undefined(result))
	{
		data = json_parse(result);
	}
}
catch (ex)
{
	promise.reject(new HttpError("Invalid JSON Response"));
	return;
}

if (status == 0 && http_status >= 200 && http_status <= 299)
{
	promise.resolve({
		status: http_status,
		data: data,
		options,
	});
}
else
{
	if (status < 0 && http_status >= 200 && http_status <= 299)
	{
		http_status = 503;
	}
		
	promise.reject({
		status: http_status,
		data: data,
		options: options,
	});
}