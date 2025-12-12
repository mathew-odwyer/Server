/// @description Initialize default parameters.

/// @instancevar {Struct.ClientConnection} connection The client connection associated with the player.

event_inherited();

/// @type {Bool}
/// @description Indicates whether the player is ready to receive notifications.
ready = false;

/// @type {Id.DsQueue}
/// @description The actions to be performed on the server.
_action_queue = ds_queue_create();

/// @type {Real}
/// @description The last performed action identifier.
_last_action_identifier = 0;

/// @description Enqueues a action to be performed.
/// @type {Struct} action The action to be performed.
enqueue_action = function(action)
{
    ds_queue_enqueue(_action_queue, action);
}

/// @description Enqueues a collection of actions to be performed.
/// @param {Array} actions The collection of actions to be performed.
enqueue_actions = function(actions)
{
    array_foreach(actions, function(action)
    {
        enqueue_action(action);
    });
}

/// @description Sends a JSON-RPC 2.0 notification to the `player`.
/// @param {String} procedure The procedure to send to the `player`.
/// @param {Any} params The parameters of the procedure to send to the `player`
notify = function(procedure, params)
{
	/// @feather ignore once GM1041
    obj_server.notify(connection, procedure, params);
}

/// @type {Function}
/// @description Processes all player actions for the current tick.
_process_tick = function()
{
    notify("player.reconcile", {
        x: x,
        y: y,
        last_identifier: _last_action_identifier,
    });

    with (obj_player)
    {
        if (self.id == other.id)
        {
            continue;
        }

        notify("player.action_remote", {
            name: other.name,
            actions: other._actions_performed,
        });
    }
}
