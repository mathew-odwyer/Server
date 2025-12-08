/// @description Reconcile player actions.

alarm[0] = tick_rate;

var length = array_length(_actions_performed);

// If no actions have been performed since the last time, no need to send updates.
if (length == 0)
{
    exit;
}

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

    notify("player.move_remote", {
        name: other.name,
        actions: other._actions_performed,
    });
}

array_resize(_actions_performed, 0);
