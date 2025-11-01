/// @description Draw the character.

// TODO: Maybe use the on_draw state for the customizer.draw
//			- This way the server doesn't have to draw if they don't want too.
customizer.draw(image_index, x, y);
_state_machine.run_state(state_type.on_draw);
