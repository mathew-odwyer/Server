/// @description Provides an error that is thrown when an RPC error occurs.
/// @param {String} message The message that describes the error.
/// @param {Struct|Undefined} inner_error The inner error that was thrown (if any).
function RpcError(message, inner_error = undefined) : Error(message, inner_error) constructor
{
}