/// @description Sends a asynchronous HTTP Request.
/// @param {String} url The URL of the request.
/// @param {String} type The method type (POST, GET, etc).
/// @param {Struct} body The body to send (empty if none).
/// @param {Struct} options The options (headers, etc).
/// @returns {Struct.Promise} Returns the promise to be resolved by the HTTP client.
function http_async(url, type, body = {}, options = undefined)
{
	static RequestToContextMap = ds_map_create();
	
	if (is_undefined(options))
	{
		options = {
			headers: ds_map_create(),
		};
		
		options.headers[? "Content-Type"] = "application/json";
	}
	
	var promise = new Promise();
	var request = http_request(url, type, options.headers, json_stringify(body));
	
	RequestToContextMap[? request] = {
		promise: promise,
		options: options,
	};
	
	ds_map_destroy(options.headers);
	struct_remove(options, "headers");
	
	return promise;
}

/// @description Provides functionality for registering and invoking HTTP body parsers based on `Content-Type` values.
function HttpBodyParser() constructor
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(HttpBodyParser));
	
	/// @type {DsMap<String, Function>}
	/// @description The content type to parser function map.
	static _type_to_parser_map = ds_map_create();
	
	/// @description Registers a parser function for a specific `content_type`.
	/// @param {String} content_type The MIME type to associate with the parser (case-insensitive).
	/// @param {Function} parser The parser function that will handle the specified `content_type`.
	static Add = function(content_type, parser)
	{
		content_type = string_lower(content_type);
		_type_to_parser_map[? content_type] = parser;
	}
	
	/// @description Checks whether a parser exists for the specified `content_type`.
	/// @param {String} content_type The MIME type to check (case-insensitive).
	/// @returns {Bool} Returns `true` if a parser is registered for the `content_type`; otherwise, `false`.
	static Has = function(content_type)
	{
		return ds_map_exists(_type_to_parser_map, content_type);
	}
	
	/// @description Parses the given `body` using the parser associated with the specified `content_type`.
	/// @param {String} content_type The MIME type used to determine which parser to use.
	/// @param {Any} body The raw body data to be parsed.
	/// @returns {Any} The parsed result, or an empty struct `{}` if no parser is registered for the `content_type`.
	static Parse = function(content_type, body)
	{
		var type = string_lower(content_type);
		
		if (!Has(type))
		{
			_logger.log(log_type.warning, $"Failed to parse content for type: '{content_type}'");
			return {};
		}
		
		return _type_to_parser_map[? type](body);
	}
}

new HttpBodyParser();

HttpBodyParser.Add("application/json",json_parse);
HttpBodyParser.Add("application/json; charset=utf-8", json_parse);
HttpBodyParser.Add("application/problem+json; charset=utf-8", json_parse);