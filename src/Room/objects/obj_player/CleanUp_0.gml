/// @description Cleanup resources.

with (obj_player)
{
    if (self.identifier == other.identifier)
    {
        continue;
    }

    notify("room.player.leave", { identifier: other.identifier });
}
