/// @description Reconcile player actions.

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
alarm[0] = tick_rate;