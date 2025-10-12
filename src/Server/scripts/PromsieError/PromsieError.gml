/// @description Represents an error that is thrown when a promise error occurs.
/// @param {String} message The message that describes the error.
/// @param {Struct|Undefined} inner_error The inner error that was thrown (if any).
function PromsieError(message, inner_error) : Error(message, inner_error) constructor
{
}