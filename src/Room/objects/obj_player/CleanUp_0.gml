/// @description Cleanup resources.

// with (obj_player)
// {
//     if (self.id == other.id)
//     {
//         continue;
//     }

//     notify("player.delete_remote", other.name);
// }

ds_queue_destroy(_action_queue);
