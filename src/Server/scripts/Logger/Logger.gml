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
/// @argument {Function} log_func The function used when logging.
function Logger() constructor
{
    /// @type {Enum.log_type}
    /// @description The logging level.
	static LogLevel = log_type.debug;

    /// @type {Function}
    /// @description The function used when logging.
    static LogFunction = show_debug_message;

    /// @description Logs a message.
    /// @argument {Enum.log_type} type The type of message to log.
    /// @argument {String} text The message to log.
    static Log = function(type, text)
    {
		if (type < LogLevel)
        {
            return;
        }
        
        LogFunction($"[{_get_log_type(LogLevel)}]: {text}");
    }

    /// @description Converts the specified `type` to a readable `string`.
    /// @argument {Enum.log_type} type The log type to convert.
    /// @returns {String} The specified `type` converted to a readable `string`.
    static _get_log_type = function(type)
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

new Logger();