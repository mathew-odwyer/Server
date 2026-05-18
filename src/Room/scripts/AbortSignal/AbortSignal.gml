/// @description Represents a signal object that allows you to communicate with an asynchronous operation (such as a fetch request) and abort.
function AbortSignal() constructor
{
    /// @type {Bool}
    /// @description Indicates whether the operation has been aborted.
    _aborted = false;

    /// @type {String|Undefined}
    /// @description The reason the operation has been aborted (or undefined).
    _reason = undefined;

    /// @description Aborts the operation associated with the specified `signal`.
    /// @param {Struct.AbortSignal} signal The signal associated with the operation to be aborted.
    /// @param {String|Undefined} reason The reason the operation has been aborted (or undefined).
    static Abort = function(signal, reason = undefined)
    {
        signal._aborted = true;
        signal._reason = reason;
    }

    /// @description Gets a value indicating whether the operation has been aborted.
    /// @returns {Bool} Returns `true` if the operation has been aborted; otherwise, `false`.
    get_aborted = function()
    {
        return _aborted;
    }

    /// @description Gets the reason the operation has been aborted (or undefined).
    /// @returns {String|Undefined} Returns the reason the operation has been aborted (or undefined).
    get_reason = function()
    {
        return _reason;
    }
}
