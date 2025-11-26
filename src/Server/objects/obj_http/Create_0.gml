/// @description Initialize default paramters.

instance_singleton(obj_http);

/// @type {Struct.Logger}
/// @description The logger.
_logger = new Logger(nameof(obj_http));

/// @description Converts a Problem Details object to a JSON-RPC error.
/// @param {Real} status The HTTP status code.
/// @param {Struct} error The Problem Details to convert.
/// @returns {Struct} Returns the JSON-RPC error object.
_convert_problem_details_to_jsonrpc = function(status, error)
{
	if (status == 200)
	{
		// GM will return a 200 OK when the server might not have responded.
		// If that happens, let's make sure we set the status to 503 - Service Unavailable.
		status = 503;
	}
	
	return {
		code: error[$ "status"] ?? status,
		message: error[$ "title"] ?? "Internal Server Error",
		data: {
			detail: error[$ "detail"] ?? "An unexpected error occurred. Please try again later.",
			errors: error[$ "errors"] ?? [],
		},
	};
}
