/// @description Initialize default parameters.

/// @description Enumerates the available character states. 
enum character_state
{
    idle,
    walk,
	wander,
};

/// @type {String}
/// @description The name of the character. 
name = "";

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

/// @description Moves the character based on the current input.
/// @param {Real} move_x The horizontal movement: (+) right, (-) left.
/// @param {Real} move_y The vertical movement: (+) down, (-) up
_move = function(move_x, move_y)
{
	// If moving diagonally, normalize the movement vector.
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
	
	move_and_collide(move_x, move_y, collidables);
}

/// @description Determines whether the character is currently moving.
/// @returns {Bool} Returns `true` if the character is moving; otherwise, `false`.
_moving = function()
{
    return _move_x < 0 || _move_x > 0 || _move_y < 0 || _move_y > 0;
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
	
	_animation_system.set_animation($"{state}{tag}");
}

/// @description Handles the idle state.
/// @param {Enum.state_type} type The state type.
_idle_state = function(type)
{
	switch (type)
	{
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

_state_machine.add_state("Idle", character_state.idle, _idle_state);
_state_machine.add_state("Walk", character_state.walk, _walk_state);

_state_machine.set_state(character_state.idle);