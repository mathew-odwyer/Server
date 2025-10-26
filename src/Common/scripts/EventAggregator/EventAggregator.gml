/// @description Represents an event aggregator (pub-sub).
function EventAggregator() constructor
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(EventAggregator));
	
    /// @type {Array<Function>}
    /// @description Holds events and their subscribers.
    static _events = {};

    /// @description Publishes an event with the given `name` and `data` to its subscribers.
    /// @param {String} name The `name` of the event to publish.
    /// @param {Any} data The `data` associated with the event.
    static Publish = function(name, data = {})
    {
		_logger.log(log_type.trace, $"Publishing '{name}'");
		
        var subscribers = struct_get(_events, name);

        if (is_undefined(subscribers))
        {
            return;
        }
    
        var length = array_length(subscribers);
    
        for (var i = 0; i < length; i++)
        {
            var subscriber = subscribers[i];
    
            if (is_undefined(subscriber))
            {
                continue;
            }
    
            subscriber.callback(data);
        }
    }

    /// @description Subscribes the specified `callback` to the specified `event`.
    /// @param {Any} identifier The `identifier` for the subscription.
    /// @param {String} name The `name` of the event to subscribe to.
    /// @param {Function} callback The function to subscribe to the `event`.
    static Subscribe = function(identifier, name, callback)
    {
		_logger.log(log_type.trace, $"'{identifier} is subscribing to '{name}'...");
		
        if (!struct_exists(_events, name))
        {
            struct_set(_events, name, []);
        }
    
        var callbacks = struct_get(_events, name);
        array_push(callbacks, { identifier, callback });
    }

	/// @description Unsubscribes a specific `identifier` from a given event.
	/// @param {Any} identifier The `identifier` for the subscription to remove.
	/// @param {String} name The `name` of the event to unsubscribe from.
	static Unsubscribe = function(identifier, name)
	{
	    var subscribers = struct_get(_events, name);
    
	    if (is_undefined(subscribers))
	    {
	        return;
	    }

		_logger.log(log_type.trace, $"'{identifier}' is unsubscribing from '{name}'...");

	    var filtered_subscribers = [];
	    var length = array_length(subscribers);
    
	    for (var i = 0; i < length; i++)
	    {
	        var subscriber = subscribers[i];
        
	        if (subscriber.identifier != identifier)
	        {
	            array_push(filtered_subscribers, subscriber);
	        }
	    }

	    struct_set(_events, name, filtered_subscribers);
	}

	/// @description Unsubscribes all events associated with the given `identifier`.
	/// @param {Any} identifier The `identifier` for the subscriptions to remove.
	static UnsubscribeAll = function(identifier)
	{
	    var keys = struct_get_names(_events);
		var length = array_length(keys);

	    for (var i = 0; i < length; i++)
	    {
	        var name = keys[i];
	        Unsubscribe(identifier, name);
	    }
	}
}

new EventAggregator();