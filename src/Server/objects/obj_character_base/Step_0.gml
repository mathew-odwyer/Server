/// @description Process movement.

// Lock movement when we're wandering or can't walk.
if (_moving() && can_wander || !can_walk)
{
	_move_x = 0;
	_move_y = 0;
}

_state_machine.run_state(state_type.on_step);
_move(_move_x, _move_y);