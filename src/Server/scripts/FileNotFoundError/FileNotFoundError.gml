/// @description Represents an error that is thrown when a file couldn't be located.
/// @param {String} file_path The path of the file that couldn't be located.
/// @param {Struct|Undefined} inner_error The inner error that was thrown (if any).
function FileNotFoundError(file_path, inner_error = undefined)
	: Error($"The specified '{nameof(file_path)}' couldn't be located: '{file_path}'", inner_error) constructor
{
}
