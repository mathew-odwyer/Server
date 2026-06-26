/// @description Initialize default paramters.

/// @type {String}
/// @description The API url.
#macro api_url environment_get_variable("CLIENTOPTIONS__BASEURL")

/// @type {String}
/// @description The API key.
#macro api_key environment_get_variable("CLIENTOPTIONS__APIKEY")

instance_singleton(obj_http);

/// @type {Struct.Logger}
/// @description The logger.
_logger = new Logger(nameof(obj_http));
