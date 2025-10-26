/// @description Initialize default parameters.

/// @type {Struct.CharacterCustomizer}
/// @description Represents the character customizer.
customizer = new CharacterCustomizer();

/// @type {Struct.SnowState}
/// @description The state machine.
_state_machine = new SnowState("idle");

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

/// @description Gets the direction index.
/// @returns {Real} Returns the direction index.
_get_direction = function()
{
	/// @type {Real}
	/// @description The last known direction (defaults to down).
	static _last_direction = 3;
	
	if (_move_x == 0 && _move_y == 0)
	{
		return _last_direction;
	}
	
	// Compute index based on the direction the character is moving.
	var angle = point_direction(0, 0, _move_x, _move_y);
	_last_direction = floor((angle + 45) / 90) % 4;
	
	return _last_direction;
}

_update_animation = function()
{
	static _tags = ["Right", "Up", "Left", "Down"];
	
	var state = _state_machine.get_current_state();
	var tag = _tags[_get_direction()];
	var name = $"{state}{tag}";
	
	_animation_system.set_animation(name);
}

_state_machine.add("idle", {
	enter: _update_animation,
	step: function() {
		if (_moving())
		{
			_state_machine.change("walk");
		}
	},
});

_state_machine.add("walk", {
	enter: _update_animation,
	step: function() {
		if (!_moving())
		{
			_state_machine.change("idle");
		}
	},
});
