/// @description Cleanup resources.

with (obj_player)
{
    if (self.identifier == other.identifier)
    {
        continue;
    }

    notify("player.delete_remote", other.name);
}

ds_queue_destroy(_action_queue);