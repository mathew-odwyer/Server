function http_parse_error(error)
{
	static _default_error = {
		title: "Internal Server Error",
		message: "An unexpected error occurred. Please try again later.",
	};
	
	if (!is_struct(error))
	{
		return _default_error;
	}
	
	var title = struct_get(error, "title") ?? _default_error.title;
	var detail = struct_get(error, "detail") ?? _default_error.error;
	var errors = struct_get(error, "errors");
	
	if (!is_struct(errors))
	{
		return {
			title: title,
			message: detail,
		};
	}
	
	var names = struct_get_names(errors);
	var length = array_length(names);

	if (length == 0)
	{
		return _default_error;
	}
	else if (length == 1)
	{
		return {
			title: title,
			message: $"{names[i]}: {struct_get(errors, names[0])}",
		};
	}
	
	var content = "";
	
	for (var i = 0; i < length; i++)
	{
		var name = names[i];
		var values = struct_get(errors, name);
		
		if (!is_array(values) || array_length(values) == 0)
		{
			continue;
		}
		
		content += $"{name}:\n";
		
		for (var j = 0; j < array_length(values); j++)
		{
			var value = values[j];
			
			if (is_string(value) && !string_empty(value))
			{
				content += $"    - {value}\n";
			}
		}
		
		content += "\n";
	}
	
	return {
		title: title,
		message: string_trim(content),
	};
}