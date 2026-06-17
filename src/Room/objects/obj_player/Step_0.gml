/// @description Process action queue.

var length = array_length(_action_queue);

if (length != 0)
{
    var action = _action_queue[0];

    if (is_struct(action))
    {
    	var type = action[$ "Type"];
        var identifier = action[$ "Identifier"];
        
        if (!is_string(type) || !is_real(identifier))
        {
            exit;
        }

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

event_inherited();
