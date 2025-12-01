/// @description Parses a JSON-RPC 2.0 data object that contains a HTTP response.
/// @param {Struct} error The JSOn-RPC 2.0 error object.
/// @returns {String} Returns a string containing the error information.
function jsonrpc_error_parse(error)
{
    /// @type {Struct.Logger} 
    /// @description The logger.
    static _logger = new Logger(nameof(jsonrpc_error_parse));

    if (!is_struct(error) || !struct_exists(error, "data"))
    {
        _logger.log(log_type.warning, $"Failed to parse JSON-RPC error for data: {json_stringify(data, debug_mode)}");
        return "An unexpected error occurred. Please try again later.";
    }
	
	var data = error[$ "data"];

    // If detail exists, use that over errors.
    if (struct_exists(data, "detail"))
    {
        return data[$ "detail"];
    }

    if (!struct_exists(data, "errors") || !is_struct(data[$ "errors"]))
    {
        _logger.log(log_type.warning, $"Failed to parse JSON-RPC error for data: {json_stringify(data, debug_mode)}");
        return "An unexpected error occurred. Please try again later.";
    }

    var result = "";
	
	var errors = data[$ "errors"];
    var names = struct_get_names(errors);

    for (var i = 0; i < array_length(names); i++)
    {
        var name = names[i];
        var messages = struct_get(errors, name);

        if (!is_array(messages) || !is_string(name))
        {
            continue;
        }

        result += $"{name}\n";

        for (var j = 0; j < array_length(messages); j++)
        {
            var message = messages[j];

            if (!is_string(message))
            {
                continue;
            }

            result += $"    - {message}\n";
        }
    }

    return result;
}
