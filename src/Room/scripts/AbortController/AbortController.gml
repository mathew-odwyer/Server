/// @description Provides a mechanism to signal and handle the cancellation of asynchronous operations.
function AbortController() constructor
{
	/// @type {Struct.Logger}
	/// @description The logger.
	
    /// @type {Any}
    /// @description The signal which can be used to communicate with, or to abort, an asynchronous operation.
    signal = new AbortSignal();

    /// @description Signals that the operation has been aborted.
    /// @param {String|Undefined} reason The reason the operation has been aborted (or undefined).
    abort = function(reason = undefined)
    {
		_logger.log(log_type.trace, $"Asynchronous operation aborted: '{reason}'");
        AbortSignal.Abort(signal, reason);
    }
}
