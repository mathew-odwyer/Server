/// @description Represents the eventual completion (or failure) of an asynchronous operation and its resulting value.
/// @param {Real} timeout The amount of time to pass in milliseconds before the promise is rejected.
function Promise() constructor
{
	/// @type {Function}
	/// @description A global handler for unhandled rejections.
	static UnhandledRejectionCallback = undefined;
	
	/// @type {String}
	/// @description The current state of the promise.
    _state = "pending";
	
	/// @type {Any}
	/// @description The resolve value or rejection reason.
    _value = undefined;
	
	/// @type {Struct.Promise|Undefined}
	/// @description The parent.
	_parent = undefined;
	
	/// @type {Array}
	/// @description The handlers of the promise.
	_handlers = [];
	
	/// @type {Bool}
	/// @description Indicates whether the promise has been rejected and handled.
	_handled = false;
	
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

		var chain = _handlers;
		var length = array_length(chain);

        for (var i = 0; i < length; i++)
        {
            var link = chain[i];
            var promise = link.promise;
            var callback = link.fulfilled;

            try
            {
                var result = callback(_value);
				
				/// @feather ignore GM1041
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
							promise._handled = true;
							promise.reject(error);
						}));
                }
                else
                {
					 // No fulfilled (synchronous); forward value to child.
                    promise.resolve(result);
                }
            }
            catch (ex)
            {
                promise.reject(ex);
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
		
		var chain = _handlers;
		var length = array_length(chain);

        for (var i = 0; i < length; i++)
        {
            var link = chain[i];
            var promise = link.promise;
            var callback = link.rejected;

            try
            {
                var result = callback(_value);

				// If the result is another promise, chain it.
				/// @feather ignore GM1041
                if (is_instanceof(result, Promise))
                {
                    result
                        .next(method({promise}, function(value)
						{
							promise.resolve(value);
						}))
                        .fail(method({promise}, function(error)
						{
							promise._handled = true;
							promise.reject(error);
						}));
                }
                else
                {
                    // error handler returned a non-promise so let's treat it as resolved.
					// No rejected (synchronous); forward value to child.
                    promise.resolve(result);
                }
            }
            catch (err)
            {
                promise.reject(err);
            }
        }
		
		// If no .fail() ever handled this, call global unhandled rejection
	    if (!is_undefined(UnhandledRejectionCallback))
	    {
			var this = self;
			
			call_later(1, time_source_units_frames, method({this, _value}, function()
			{
				if (this._handled)
				{
					return;
				}
				
				// Walk ancestors to see if any parent has been marked handled
				var current = this._parent;
				
				while (!is_undefined(current))
		        {
		            if (current._handled)
					{
						// If we have handnled it already, forget about it.
						return;
					}
					
		            current = current._parent;
		        }
				
				// Finally, it must be unhandled.
				Promise.UnhandledRejectionCallback(_value);
			}));
	    }
    }

    /// @description Registers a callback to be invoked when the `Promise` is resolved.
    /// @param {Function} callback The function to invoke when the `Promise` is resolved.
    /// @returns {Struct.Promise} A new `Promise` that resolves with the result of the callback or rejects if an error occurs.
    next = function(callback)
    {
        var promise = new Promise();
		promise._parent = self;
		
        if (_state == "pending")
        {
			array_push(_handlers, {
				promise: promise,
				fulfilled: callback,
				rejected: function(reason) {
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
							promise._handled = true;
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
				promise._handled = true;
                promise.reject(err);
            }
        }
        else if (_state == "rejected")
        {
			// If we've already rejected and synchronous just reject.
			promise._handled = true;
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
		promise._parent = self;
		
        if (_state == "pending")
        {
			array_push(_handlers, {
				promise: promise,
				fulfilled: function(value) {
					promise.resolve(value);
				},
				rejected: callback
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
                        .next(method({promise}, function(value)
						{
							promise.resolve(value);
						}))
                        .fail(method({promise}, function(error)
						{
							promise._handled = true;
							promise.reject(error);
						}));
                }
                else
                {
					// Resolve synchronously if it's not a promise.
                    promise.resolve(result);
                }
            }
            catch (err)
            {
				promise._handled = true;
                promise.reject(err);
            }
        }	
        else if (_state == "resolved")
        {
			// Resolve synchronously if already resolved.
            promise.resolve(_value);
        }
		
		var current = self;
		
		while (!is_undefined(current))
		{
			current._handled = true;
			current = current._parent;
		}
		
        return promise;
    }
}

new Promise();
