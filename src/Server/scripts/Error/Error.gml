/// @description Represents an error that is thrown when a generic error occurs.
/// @param {String} message The message that describes the error.
/// @param {Struct|Undefined} inner_error The inner error that was thrown (if any).
function Error(message, inner_error = undefined) constructor
{
	/// @type {String}
	/// @description The message that describes the error.
	self.message = message;

	/// @type {Struct|Undefined}
	/// @description The inner error that was thrown (if any).
	self.inner_error = inner_error;
}

/// @description Logs an unhandled exception or error.
/// @param {Struct} error The error that caused the exception to be thrown.
function unhandled_exception_callback(error)
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(unhandled_exception_callback));
	_logger.log(log_type.error, $"Unhandled Error: {error}");
}
