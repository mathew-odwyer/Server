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

	var code = error[$ "status"] ?? status;
	var message = error[$ "title"] ?? "Internal Server Error";
	var data = {};

	// ASP.NET problem details for validation errors does not include the detail field.
	// So we should follow the same design decision here to eliminate confusion.
	if (struct_exists(error, "detail"))
	{
		data[$ "detail"] = error[$ "detail"];
	}
	else if (struct_exists(error, "errors"))
	{
		data[$ "errors"] = error[$ "errors"];
	}

	return {
		code: code,
		message: message,
		data: data,
	}
}
