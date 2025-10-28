/// @description Executes a callback after the given delay (in milliseconds).
/// @param {Function} callback The function to execute.
/// @param {Real} delay The delay in seconds.
/// @returns {Id.TimeSource} A timeout ID (for possible future cancellation).
function set_timeout(callback, delay)
{
	return call_later(delay, time_source_units_seconds, callback);
}
