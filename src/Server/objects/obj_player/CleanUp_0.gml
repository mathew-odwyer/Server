/// @description Cleanup resources.

event_inherited();

with (obj_player)
{
    if (self.id == other.id)
    {
        continue;
    }

    notify("player.delete_remote", other.name);
}

connection.cleanup();
