/// @description Initialize default parameters.

/// @type {Real}
/// @description The cell (or grid) width (in pixels).
#macro cell_width 16

/// @type {Real}
/// @description The cell (or grid) height (in pixels).
#macro cell_height 16

/// @type {Real}
/// @description The width of the current room (in cell space).
#macro grid_width room_width / cell_width

/// @type {Real}
/// @description The height of the current room (in cell space).
#macro grid_height room_height / cell_height


/// @description Enumerates the available character states. 
enum character_state
{
    idle,
    walk,
};

/// @type {String}
/// @description The name of the character. 
name = "";

/// @type {Struct.CharacterCustomizer}
/// @description Represents the character customizer.
customizer = new CharacterCustomizer();

/// @type {Struct.SnowState}
/// @description Represents the state machine for the character.
_state_machine = new SnowState("Idle");

/// @description Represents the character animation system.
/// @type {Struct.AnimationSystem}
_animation_system = new CharacterAnimationSystem();

/// @type {Real}
/// @description The horizontal movement: (+) right, (-) left.
_move_x = 0;

/// @type {Real}
/// @description The vertical movement: (+) down, (-) up.
_move_y = 0;

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
	static _last_direction = 3;
	
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

_update_animation = function()
{
	static tags = ["Right", "Up", "Left", "Down"];
	
	var tag = tags[_get_direction()];
	var state = _state_machine.get_current_state();
	
	_animation_system.set_animation($"{state}{tag}");
}

_state_machine.add("Idle", {
	step: function() {
		if (_moving())
		{
			_state_machine.change("Walk");
		}
		
		_update_animation();
	},
});

_state_machine.add("Walk", {
	enter: _update_animation,
	step: function() {
		if (!_moving())
		{
			_state_machine.change("Idle");
		}
		
		_update_animation();
	},
});
