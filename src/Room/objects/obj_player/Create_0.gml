/// @description Initialize default parameters.

event_inherited();

/// @type {String} 
/// @description The player identifier.
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

/// @description Forwards a JSON-RPC 2.0 notification to the player.
/// @param {String} procedure The procedure (or method) to be invoked on the client.
/// @param {Struct} params The parameters for the procedure to be invoked on the client.
notify = function(procedure, params)
{
    event_publish($"player.{identifier}.notify", {
        Method: procedure,
        Parameters: params,
    });
}

alarm[0] = tick_rate;

call_later(1, time_source_units_frames, function()
{
    event_subscribe($"player.{identifier}.action", function(event)
    {
        enqueue_actions(event[$ "ActionQueue"]);
    });
});
