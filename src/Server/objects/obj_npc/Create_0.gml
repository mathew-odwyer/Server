/// @description Initialize default parameters.

event_inherited();

EventAggregator.Subscribe(id, "PLAYER_LOGGED_IN", function(player)
{
    player.notify("npc.create_remote", {
        name: name,
        x: x,
        y: y,
    });
});

/// @type {Function}
/// @description Processes all NPC actions for the current tick.
_process_tick = function()
{
    with (obj_player)
    {
        if (self.id == other.id)
        {
            continue;
        }

        notify("npc.action_remote", {
            name: other.name,
            actions: other._actions_performed,
        });
    }
}
