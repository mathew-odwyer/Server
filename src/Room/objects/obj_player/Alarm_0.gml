/// @description Reconcile player actions.

notify("room.player.reconcile", {
    x: x,
    y: y,
    last_identifier: _last_action_identifier,
});

with (obj_player)
{
    if (self.identifier == other.identifier)
    {
        continue;
    }

    self.notify("room.player.move", {
        identifier: other.identifier,
        actions: other._actions_performed,
    });
}

array_resize(_actions_performed, 0);
alarm[0] = tick_rate;
