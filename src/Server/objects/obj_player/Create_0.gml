/// @description Initialize default parameters.

/// @instancevar {Struct.ClientConnection} connection The client connection associated with the player.

/// @type {Real}
/// @description The server-side tick rate.
#macro server_tick_rate 20

event_inherited();

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
/// @param {String} procedure The procedure to send to the `player`.
/// @param {Any} params The parameters of the procedure to send to the `player`
notify = function(procedure, params)
{
    obj_server.notify(connection, procedure, params);
}

alarm[0] = server_tick_rate;
