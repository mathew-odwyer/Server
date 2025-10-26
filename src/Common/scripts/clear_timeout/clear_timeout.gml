/// @description Cancels a timeout created by `set_timeout`.
/// @param {Id.TimeSource} timesource The timeout handle to cancel.
function clear_timeout(timesource)
{
	call_cancel(timesource);
}
