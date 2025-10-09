/// @description Provides an exception that is thrown when a JSON-RPC 2.0 error occurs
/// @param {String} message The message that describes the exception.
/// @param {Struct|Undefined} inner_exception The inner exception that was thrown (if any).
function RpcException(message, inner_exception = undefined) constructor
{
	/// @type {String}
	/// @description The message that describes the exception.
	self.message = message;

	/// @type {Struct|Undefined}
	/// @description The inner exception that was thrown (if any).
	self.inner_exception = inner_exception;
}