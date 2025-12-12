/// @description Cleanup resources.

event_inherited();

with (obj_player)
{
    notify("npc.delete_remote", name);
}

EventAggregator.UnsubscribeAll(id);
