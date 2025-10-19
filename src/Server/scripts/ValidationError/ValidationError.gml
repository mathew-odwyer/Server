/// @description Provides an error that is thrown when a validation error occurs.
/// @param {String} message The message that describes the error.
/// @param {Struct|Undefined} inner_error The inner error that was thrown (if any).
function ValidationError(message, inner_error = undefined) : RpcError(message, inner_error) constructor
{
}