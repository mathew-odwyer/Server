/// @description Represents an error that is thrown when an instanced is disposed.
/// @param {String} name The name of the instance that is disposed.
/// @param {Struct|Undefined} inner_error The inner error that was thrown (if any).
function ObjectDisposedError(name, inner_error = undefined)
	: Error($"The specified '{name}' is disposed.", inner_error) constructor
{
}
