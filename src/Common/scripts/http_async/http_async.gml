/// @description Sends a asynchronous HTTP Request.
/// @param {String} url The URL of the request.
/// @param {String} type The method type (POST, GET, etc).
/// @param {Struct} body The body to send (empty if none).
/// @param {Struct} options The options to send with this request (bearer, etc).
/// @returns {Struct.Promise} Returns the promise to be resolved by the HTTP client.
function http_async(url, type, body = {}, options = {})
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(http_async));
	
	/// @type {Id.DsMap}
	/// @description The request to context map.
	static RequestToContextMap = {};
	
	if (!instance_exists(obj_http))
	{
		instance_create_layer(0, 0, "Instances", obj_http);
	}
	
	var headers = ds_map_create();
	
	headers[? "Accept"] = "application/json, application/problem+json";
	headers[? "Content-Type"] = "application/json";
	
	if (struct_exists(options, "bearer"))
	{
		headers[? "Authorization"] = $"Bearer {options[$ "bearer"]}";
	}
	
	_logger.log(log_type.debug, $"{type} '{url}'...");
	
	var promise = new Promise();
	var request = http_request(url, type, headers, json_stringify(body));
	
	RequestToContextMap[$ request] = {
		promise: promise,
		options: options,
	};
	
	ds_map_destroy(headers);
	return promise;
}
