/// @description Represents the eventual completion (or failure) of an asynchronous operation and its resulting value.
function Promise() constructor
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(Promise));
	
	/// @type {String}
	/// @description The current state of the promise.
    _state = "pending";
	
	/// @type {Any}
	/// @description The resolve value or rejection reason.
    _value = undefined;
	
	/// @type {Array}
	/// @description The collection of resolution links.
    _resolve_chain = [];
	
	/// @type {Array}
	/// @description The collection of rejection links.
    _reject_chain = [];

    /// @description Attempts to resolve the `Promise` with the specified `value`.
    /// @param {Any} value The value used during resolution.
    resolve = function(value)
    {
		// If we've already resolved (or rejected), don't do it again.
        if (_state != "pending")
		{
			return;
		}

        _value = value;
        _state = "resolved";

		var chain = _resolve_chain;
		var length = array_length(chain);

        for (var i = 0; i < length; i++)
        {
            var link = chain[i];
            var promise = link.promise;
            var callback = link.callback;

            try
            {
                var result = callback(_value);
				
				/// @feather disable GM1041
                if (is_instanceof(result, Promise))
                {
					// If the result is another promise, chain it.
                    result
                        .next(method({promise}, function(value)
						{
							promise.resolve(value);
						}))
                        .fail(method({promise}, function(error)
						{
							promise.reject(error);
						}));
                }
                else
                {
					// Otherwise, it's not async so just resolve.
                    promise.resolve(result);
                }
            }
            catch (error)
            {
                promise.reject(error);
            }
        }
    }

    /// @description Attempts to reject the `Promise` with the specified `value`.
    /// @param {Any} value The value used during rejection.
    reject = function(value)
    {
		// If we've already resolved (or rejected), don't do it again.
        if (_state != "pending")
		{
			return;
		}

        _value = value;
        _state = "rejected";
		
		var chain = _reject_chain;
		var length = array_length(chain);
		
		if (length == 0)
		{
			_logger.log(log_type.warning, $"Unhandled Rejection: '{_value}'");
			
			if (debug_mode)
			{
				throw new PromsieError("Unhandled Rejection", _value);
			}
		}

        for (var i = 0; i < length; i++)
        {
            var link = chain[i];
            var promise = link.promise;
            var callback = link.callback;

            try
            {
                var result = callback(_value);

				// If the result is another promise, chain it.
                if (is_instanceof(result, Promise))
                {
                    result
                        .next(method({promise}, function(value)
						{
							promise.resolve(value);
						}))
                        .fail(method({promise}, function(error)
						{
							promise.reject(error);
						}));
                }
                else
                {
                    // error handler returned a non-promise so let's treat it as resolved.
                    promise.resolve(result);
                }
            }
            catch (err)
            {
                promise.reject(err);
            }
        }
    }

    /// @description Registers a callback to be invoked when the `Promise` is resolved.
    /// @param {Function} callback The function to invoke when the `Promise` is resolved.
    /// @returns {Struct.Promise} A new `Promise` that resolves with the result of the callback or rejects if an error occurs.
    next = function(callback)
    {
        var promise = new Promise();
		
        var link = {
			promise: promise,
			callback: callback
		};

        if (_state == "pending")
        {
			// If we're pending, just push to the array and wait until it's complete.
            array_push(_resolve_chain, link);
			
			// Forward the unhandled error if none is provided.
			array_push(_reject_chain, {
				promise: promise,
				callback: function(reason) {
					promise.reject(reason);
				}
			});
        }
        else if (_state == "resolved")
        {
            try
            {
				// If we've resolved already, let's handle it.
                var result = callback(_value);

				// If the result is another promise, chain it.
                if (is_instanceof(result, Promise))
                {
                    result
                        .next(method({promise}, function(value)
						{
							promise.resolve(value);
						}))
                        .fail(method({promise}, function(error)
						{
							promise.reject(error);
						}));
                }
                else
                {
					// Otherwise, it's not async so just resolve.
                    promise.resolve(result);
                }
            }
            catch (err)
            {
                promise.reject(err);
            }
        }
        else if (_state == "rejected")
        {
			// If we've already rejected and synchronous just reject.
            promise.reject(_value);
        }

        return promise;
    }

    /// @description Registers a callback to be invoked when the `Promise` is rejected.
    /// @param {Function} callback The function to invoke when the `Promise` is rejected.
    /// @returns {Struct.Promise} A new `Promise` that resolves with the result of the callback or rejects if an error occurs.
    fail = function(callback)
    {
        var promise = new Promise();
		
        var link = {
			promise: promise,
			callback: callback
		};

        if (_state == "pending")
        {
            // If we're pending, just push to the array and wait until it's complete.
            array_push(_reject_chain, link);
			
			// // Forward the unhandled success if none is provided.
			array_push(_resolve_chain, {
				promise: promise,
				callback: function(value) {
					promise.resolve(value);
				}
			});
        }
        else if (_state == "rejected")
        {
            try
            {
                var result = callback(_value);

				// If the result is a promise, chain it.
                if (is_instanceof(result, Promise))
                {
                    result
                        .next(method({promise}, function(value) { promise.resolve(value); }))
                        .fail(method({promise}, function(e) { promise.reject(e); }));
                }
                else
                {
					// Resolve synchronously if it's not a promise.
                    promise.resolve(result);
                }
            }
            catch (err)
            {
                promise.reject(err);
            }
        }
        else if (_state == "resolved")
        {
			// Resolve synchronously if already resolved.
            promise.resolve(_value);
        }

        return promise;
    }
}

