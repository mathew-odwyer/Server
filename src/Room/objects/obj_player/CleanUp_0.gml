/// @description Cleanup resources.

with (obj_player)
{
    if (self.identifier == other.identifier)
    {
        continue;
    }

    notify("room.player.leave", { identifier: other.identifier });
}

if (_action_queue != -1)
{
    ds_queue_destroy(_action_queue);
}