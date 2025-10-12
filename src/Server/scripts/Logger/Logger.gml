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
/// @argument {String} name The `name` of the instance that is logging.
/// @argument {Function} log_func The function used when logging.
function Logger(name, log_func = show_debug_message) constructor
{
	static LogLevel = log_type.debug;

    /// @type {String}
    /// @description The name of the instance or struct.
    _name = name;

    /// @type {Function}
    /// @description The function used when logging.
    _log_func = log_func;

    /// @description Logs a message.
    /// @argument {Enum.log_type} type The type of message to log.
    /// @argument {String} text The message to log.
    log = function(type, text)
    {
		if (type < LogLevel)
        {
            return;
        }
		
        _log_func($"[{_get_log_type(LogLevel)}]: [{_name}]: {text}");
    }

    /// @description Converts the specified `type` to a readable `string`.
    /// @argument {Enum.log_type} type The log type to convert.
    /// @returns {String} The specified `type` converted to a readable `string`.
    _get_log_type = function(type)
    {
        static type_to_name_map = [
		    [log_type.trace, "Trace"],
            [log_type.debug, "Debug"],
            [log_type.information, "Information"],
            [log_type.warning, "Warning"],
            [log_type.error, "Error"],
            [log_type.critical, "Critical"],
        ];

        return type_to_name_map[type][1];
    }
}
