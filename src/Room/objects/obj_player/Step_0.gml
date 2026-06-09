/// @description Process action queue.

if (!ds_queue_empty(_action_queue))
{
    var action = ds_queue_dequeue(_action_queue);

    if (!is_undefined(action) && is_struct(action))
    {
        var type = action[$ "Type"];
        var identifier = action[$ "Identifier"];

        if (!is_undefined(type) && !is_undefined(identifier))
        {
            switch (type)
            {
                case "move":
                    // Clamp movement to stop speed hacks.
                    _move_x = clamp(action[$ "MoveX"] ?? 0, -1, 1);
                    _move_y = clamp(action[$ "MoveY"] ?? 0, -1 , 1);
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
