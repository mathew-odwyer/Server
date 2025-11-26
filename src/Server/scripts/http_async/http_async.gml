/// @description Sends a asynchronous HTTP Request.
/// @param {String} url The URL of the request.
/// @param {String} type The method type (POST, GET, etc).
/// @param {Struct} body The body to send (empty if none).
/// @param {Struct} options The options to send with this request (bearer, etc).
/// @returns {Struct.__Promise} Returns the promise to be resolved by the HTTP client.
function http_async(url, type, body = {}, options = {})
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(http_async));
	
	/// @type {Id.DsMap}
	/// @description The request to context map.
	static RequestToContextMap = {};
	
	var headers = ds_map_create();
	
	headers[? "Accept"] = "application/json, application/problem+json";
	headers[? "Content-Type"] = "application/json";
	
	if (struct_exists(options, "bearer"))
	{
		headers[? "Authorization"] = $"Bearer {options[$ "bearer"]}";
	}
	
	_logger.log(log_type.debug, $"{type} '{url}'...");
	
	var request = http_request(url, type, headers, json_stringify(body));

	var promise = new Promise(method({request, options}, function(resolve, reject)
	{
		var delay = options[$ "timeout"] ?? 5;
		var timeout = call_later(delay, time_source_units_seconds, method({reject}, function()
		{
			reject(new TimeoutError("Timeout: Failed to receive response from Web API."));
		}), false);
		
		http_async.RequestToContextMap[$ request] = {
			resolve: resolve,
			reject: reject,
			timeout: timeout,
			options: options,
		};
	}));
	
	ds_map_destroy(headers);
	
	/// @feather ignore once GM1045
	return promise;
}
