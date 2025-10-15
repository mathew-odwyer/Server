/// @description Provides an error that is thrown when a user is unauthorized.
/// @param {String} message The message that describes the error.
/// @param {Struct|Undefined} inner_error The inner error that was thrown (if any).
function UnauthorizedError(message, inner_error) : RpcError(message, inner_error) constructor
{
}