/// @description Update state.

_update_animation();
_animation_system.step(id);

_state_machine.run_state(state_type.on_step_end);

// Once movement for this frame has been processed, reset.
_move_x = 0;
_move_y = 0;
_colliding = false;
