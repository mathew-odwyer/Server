function http_parse_error(error)
{
    var title = struct_get(error.data, "title") ?? "Error";
    var detail = struct_get(error.data, "detail");
    var errors = struct_get(error.data, "errors");

    var message = detail;

    // If we need to parse errors.
    if (is_struct(errors))
    {
        if (!string_empty(message))
        {
            message += "\n";
        }

        var names = struct_get_names(errors);
        var length = array_length(names);

        for (var i = 0; i < length; i++)
        {
            var name = names[i];
            var data = errors[$ name];

            if (is_undefined(name) || is_undefined(data))
            {
                continue;
            }

            if (is_array(data))
            {
                for (var j = 0; j < array_length(data); j++)
                {
                    var inner = data[j];

                    if (is_string(inner))
                    {
                        message += $"{name}: {inner}\n";
                    }
                }
            }
            else if (is_string(data))
            {
                message += $"{name}: {data}\n";
            }
        }
    }

    return {
        title: error.data.title,
        message: message,
    }
}