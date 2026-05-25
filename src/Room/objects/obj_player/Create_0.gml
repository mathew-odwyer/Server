/// @description Initialize default parameters.

event_inherited();

/// @type {String}
/// @description The name of the player
name = "Player";

/// @type {String}
/// @description The unique identifier of the player.
identifier = "";

/// @type {Id.DsQueue}
/// @description The actions to be performed on the server.
_action_queue = ds_queue_create();

/// @type {Array}
/// @description The actions performed this tick.
_actions_performed = [];

/// @type {Real}
/// @description The last performed action identifier.
_last_action_identifier = 0;

/// @description Enqueues a collection of actions to be performed.
/// @param {Array} actions The collection of actions to be performed.
enqueue_actions = function(actions)
{
    array_foreach(actions, function(action)
    {
        ds_queue_enqueue(_action_queue, action);
    });
}

/// @description Sends a JSON-RPC 2.0 notification to the `player`.
/// @param {String} method The procedure to send to the `player`.
/// @param {Any} params The parameters of the procedure to send to the `player`
notify = function(method, params)
{
    event_publish($"player.{identifier}.notify", {
        method: method,
        params: params,
    });
}

// Wait one frame so that the identifier can be set.
call_later(1, time_source_units_frames, function()
{
    // TODO: Unsubscribe from events on cleanup with event_unsubscribe.
    event_subscribe($"player.{identifier}.action", enqueue_actions);
});

alarm[0] = tick_rate;
