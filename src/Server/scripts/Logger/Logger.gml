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
		
		static _type_to_name_map = [
		    [log_type.trace, "Trace"],
            [log_type.debug, "Debug"],
            [log_type.information, "Information"],
            [log_type.warning, "Warning"],
            [log_type.error, "Error"],
            [log_type.critical, "Critical"],
        ];
        
        LogFunction($"[{_type_to_name_map[LogLevel][1]}]: {text}");
    }
}

new Logger();