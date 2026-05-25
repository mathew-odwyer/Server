/// @description Enumerates the available message log types.
enum log_type
{
    trace = 0,
    debug = 1,
    information = 2,
    warning = 3,
    error = 4,
    critical = 5,
};

/// @description Represents a logger.
/// @param {String} name The name of the instance logging.
function Logger(name = "global") constructor
{
    /// @type {Enum.log_type}
    /// @description The logging level.
	static LogLevel = log_type.debug;

    /// @type {Function}
    /// @description The function used when logging.
    static LogFunction = show_debug_message;
	
	/// @description Gets the log file path for all loggers.
	/// @returns {String} Returns the log file path for all loggers.
	static GetLogPathCallback = function()
	{
		var now = date_current_datetime();
		var str = date_datetime_string(now);
		
		str = string_replace_all(str, "/", "-");
		str = string_replace_all(str, ":", "-");
		
		var path = $"Logs/{str}.txt";
		
		return path;
	}
	
	/// @type {String}
	/// @description The log file path.
	static _path = GetLogPathCallback();
	
	/// @type {String}
	/// @description The name of the instance logging.
	_name = name;

    /// @description Logs a message.
    /// @argument {Enum.log_type} type The type of message to log.
    /// @argument {String} text The message to log.
    log = function(type, text)
    {
		if (type < LogLevel)
        {
            return;
        }
		
		// ANSI Colour Formatting.
		static _type_to_name_map = [
		    [log_type.trace, "\u001b[37m", "trce"], // white
		    [log_type.debug, "\u001b[36m", "debug"], // cyan
		    [log_type.information, "\u001b[32m", "info"], // green
		    [log_type.warning, "\u001b[33m", "warn"], // yellow
		    [log_type.error, "\u001b[31m", "fail"], // red
		    [log_type.critical, "\u001b[35m", "crit"], // magenta
		];

		var reset = "\u001b[0m";
		var color = _type_to_name_map[type][1];
		var label = _type_to_name_map[type][2];
		
		var now = date_current_datetime();
		
		var year = string(date_get_year(now));
		var month = string_format(date_get_month(now), 2, 0);
		var day = string_format(date_get_day(now), 2, 0);
		
		var hour = string_format(date_get_hour(now), 2, 0);
		var minute = string_format(date_get_minute(now), 2, 0);
		var second = string_format(date_get_second(now), 2, 0);
		var ms = string_format((now mod 1) * 1000, 3, 0);

		// Replace spaces from string_format padding with zeroes.
		month = string_replace_all(month, " ", "0");
		day = string_replace_all(day, " ", "0");
		
		hour = string_replace_all(hour, " ", "0");
		minute = string_replace_all(minute, " ", "0");
		second = string_replace_all(second, " ", "0");
		ms = string_replace_all(ms, " ", "0");

		var date_time = $"{year}/{month}/{day} {hour}:{minute}:{second}.{ms}";

		var entry_console = $"{date_time} {color}{label}{reset}: {_name}\n      {text}";
		var entry_file = $"{date_time} {label}: {_name}\n      {text}";

		LogFunction(entry_console);

		// Write the clean version to file
		var file = file_text_open_append(_path);
		file_text_write_string(file, entry_file);
		file_text_write_string(file, "\n");
		file_text_close(file);
    }
}

new Logger();