/// @description Logs an unhandled exception or error.
/// @param {Struct} error The error that caused the exception to be thrown.
function unhandled_exception_callback(error)
{
	show_debug_message("--------------------------------------------------------------");
    show_debug_message($"Unhandled Error {error}");
    show_debug_message("--------------------------------------------------------------");
}
