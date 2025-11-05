/// @description Update required animations and chunk loading.

WorldManager.LoadChunk(x, y);

if (!ds_queue_empty(_action_queue))
{
    var action = ds_queue_dequeue(_action_queue);   

    if (!is_undefined(action) && is_struct(action))
    {
        var type = action[$ "type"];
        var identifier = action[$ "identifier"];

        if (!is_undefined(type) && !is_undefined(identifier))
        {
            switch (type)
            {
                case "move":
                    // Clamp movement to stop speed hacks.
                    _move_x = clamp(action[$ "move_x"] ?? 0, -1, 1);
                    _move_y = clamp(action[$ "move_y"] ?? 0, -1 , 1);
                    break;

                default:
                    break;
            }

            _last_action_identifier = identifier;
            array_push(_actions_performed, action);
        }
    }
}

event_inherited();
