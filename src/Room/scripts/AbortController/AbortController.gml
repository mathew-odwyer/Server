/// @description Provides a mechanism to signal and handle the cancellation of asynchronous operations.
function AbortController() constructor
{
    /// @type {Any}
    /// @description The signal which can be used to communicate with, or to abort, an asynchronous operation.
    signal = new AbortSignal();

    /// @description Signals that the operation has been aborted.
    /// @param {String|Undefined} reason The reason the operation has been aborted (or undefined).
    abort = function(reason = undefined)
    {
		show_debug_message($"Asynchronous operation aborted: '{reason}'");
        AbortSignal.Abort(signal, reason);
    }
}
