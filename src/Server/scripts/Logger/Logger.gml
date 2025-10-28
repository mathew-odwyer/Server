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
function Logger(name) constructor
{
    /// @type {Enum.log_type}
    /// @description The logging level.
	static LogLevel = log_type.debug;

    /// @type {Function}
    /// @description The function used when logging.
    static LogFunction = show_debug_message;
	
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
		
		static _type_to_name_map = [
		    [log_type.trace, "Trace"],
            [log_type.debug, "Debug"],
            [log_type.information, "Information"],
            [log_type.warning, "Warning"],
            [log_type.error, "Error"],
            [log_type.critical, "Critical"],
        ];
        
        LogFunction($"[{_type_to_name_map[type][1]}] [{_name}]: {text}");
    }
}
