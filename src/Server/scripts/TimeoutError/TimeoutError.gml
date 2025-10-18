/// @description Represents an error that is thrown when a timeout occurs.
/// @param {String} message The message that describes the error.
/// @param {Struct|Undefined} inner_error The inner error that was thrown (if any).
function TimeoutError(message, inner_error) : Error(message, inner_error) constructor
{
}