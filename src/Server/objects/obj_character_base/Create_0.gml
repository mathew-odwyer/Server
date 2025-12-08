/// @description Initialize default parameters.

/// @instancevar {Bool} can_walk Indicates whether the character can walk.
/// @instancevar {Bool} can_wander Indicates whether the character can wander.
/// @instancevar {Real} wander_min_wait_frames The minimum time the character must wait before wandering.
/// @instancevar {Real} wander_max_wait_frames The maximum time the character must wait before wandering.
/// @instancevar {Real} wander_radius How far the character can wander (in pixels).

/// @description Enumerates the available character states. 
enum character_state
{
    idle,
    walk,
	wander,
};

/// @type {Struct.CharacterCustomizer}
/// @description The character customizer.
customizer = new CharacterCustomizer();

/// @type {Struct.StateMachine}
/// @description The state machine for the character.
_state_machine = new StateMachine();

/// @description The character animation system.
/// @type {Struct.AnimationSystem}
_animation_system = new CharacterAnimationSystem();

/// @type {Real}
/// @description The horizontal movement: (+) right, (-) left.
_move_x = 0;

/// @type {Real}
/// @description The vertical movement: (+) down, (-) up.
_move_y = 0;

/// @type {Real}
/// @description The last direction as an interger.
_last_direction = 3;

/// @type {Asset.GMPath}
/// @description The path used for wandering.
_path = path_add();

_colliding = false;

/// @description Moves the character based on the current input.
/// @param {Real} move_x The horizontal movement: (+) right, (-) left.
/// @param {Real} move_y The vertical movement: (+) down, (-) up
_move = function(move_x, move_y)
{
	if (move_x != 0 && move_y != 0)
	{
		// Normalize diagonal movement to prevent faster movement.
		var diag_modifier = 1 / sqrt(2);

		move_x *= diag_modifier;
		move_y *= diag_modifier;
	}

	var collidables = [obj_entity_base];

	if (layer_exists("Collisions"))
	{
		var collisions_layer = layer_get_id("Collisions");
	    var tilemap_id = layer_tilemap_get_id(collisions_layer);

	    array_push(collidables, tilemap_id);
	}
	
	_colliding = array_length(move_and_collide(move_x, move_y, collidables)) > 0;
}

/// @description Determines whether the character is currently moving.
/// @returns {Bool} Returns `true` if the character is moving; otherwise, `false`.
_moving = function()
{
    return _move_x !=0 || _move_y != 0;
}

/// @description Gets the current direction index for the character.
/// @returns {Real} Returns an interger that represents the current direction; mapped 0-3.
_get_direction = function()
{
	if (_move_x == 0 && _move_y == 0)
	{
		return _last_direction;
	}
	
	// Get the direction vector for the current direction.
	var angle = point_direction(0, 0, _move_x, _move_y);

	// Then, converts the direction vector to an integer based on the angle.
	// The integers are represented counter-clockwise (0 = right, 1 = up, 2 = left, 3 = down).
	var index = floor(((angle + 45) % 360) / 90);
	
	_last_direction = index;
	return _last_direction;
}

/// @descriptions Updates the animation for the character based on the state and direction.
_update_animation = function()
{
	static tags = ["Right", "Up", "Left", "Down"];
	
	var tag = tags[_get_direction()];
	var state = _state_machine.get_name();
	
	if (state == "Wander")
	{
		// Wandering and walking are the same animation.
		state = "Walk";
	}
	
	_animation_system.set_animation($"{state}{tag}");
}

/// @description Handles the idle state.
/// @param {Enum.state_type} type The state type.
_idle_state = function(type)
{
	switch (type)
	{
		case state_type.on_enter:
			if (can_wander)
			{
				var wait_frames = irandom_range(wander_min_wait_frames, wander_max_wait_frames);
				
				call_later(wait_frames, time_source_units_frames, function()
				{
					_state_machine.set_state(character_state.wander);
				});
			}
			break;

		case state_type.on_step:
			if (_moving())
			{
				_state_machine.set_state(character_state.walk);
			}
			break;
	}
}

/// @description Handles the walk state.
/// @param {Enum.state_type} type The state type.
_walk_state = function(type)
{
	switch (type)
	{
		case state_type.on_step:
			if (!_moving())
			{
				_state_machine.set_state(character_state.idle);
			}
			break;
	}
}

/// @description Handles the wandering state.
/// @param {Enum.state_type} type The state type.
_wander_state = function(type)
{
	switch (type)
	{
		case state_type.on_enter:
			obj_ai_manager.generate_random_path(_path, x, y, xstart, ystart, wander_radius);
			break;
			
		case state_type.on_exit:
			path_clear_points(_path);
			break;

		case state_type.on_step:
			// Get the next point in the path (in room coordinates).
			var point_x = path_get_point_x(_path, 0);
			var point_y = path_get_point_y(_path, 0);
			
			// Get the distance between the current position and the next point.
			var distance = point_distance(x, y, point_x, point_y);
			
			// If we're still moving along the path, keep moving.
			if (distance > 1)
			{
				// Get the direction of the vector (where we want to go).
				var heading = point_direction(x, y, point_x, point_y);
				
				// Normalize the vector
				// Also floor it to ensure we only move in the correct direction each frame.
				_move_x = lengthdir_x(1.0, heading);
				_move_y = lengthdir_y(1.0, heading);
			}
			else
			{
				// If we've reached the end of this point in the path, go to the next point.
				path_delete_point(_path, 0);
			}
			break;
			
		case state_type.on_step_end:
			if (_colliding || path_get_number(_path) == 0)
			{
				// Since we're only wandering, to save on CPU - go back to idle if we collide with anything.
				// There's no need to try again or anything.
				_state_machine.set_state(character_state.idle);
			}
			break;
	}
}

_state_machine.add_state("Idle", character_state.idle, _idle_state);
_state_machine.add_state("Walk", character_state.walk, _walk_state);
_state_machine.add_state("Wander", character_state.wander, _wander_state);

_state_machine.set_state(character_state.idle);
