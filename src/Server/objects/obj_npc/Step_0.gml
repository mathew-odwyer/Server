/// @description Enqueue NPC actions.

event_inherited();

if (_moving())
{
    array_push(_actions_performed, {
        type: "move",
        move_x: _move_x,
        move_y: _move_y,
    });
}
