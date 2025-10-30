/// @description Enumerates the available states within a finite state machine.
enum state_type
{
    on_enter,
    on_exit,
    on_step,
	on_step_end,
    on_draw,
}

/// @description Represents a state machine.
function StateMachine() constructor
{
    /// @type {Any}
    /// @description Represents the current state.
    _state = undefined;

    /// @type {Array}
    /// @description Represents all possible states.
    _states = {};

    /// @description Adds a state to the state machine.
    /// @param {String} name The name of the state.
    /// @param {Any} state The state identifier.
    /// @param {Function} action The action associated with the state.
    add_state = function(name, state, action)
    {
        _states[$ string(state)] = {
            name: name,
            action: action,
        };
    }

    /// @description Sets the current state of the state machine.
    /// @param {Any} state The state to set.
    set_state = function(state)
    {
        var code = string(state);

        if (!struct_exists(_states, code))
        {
			return;
        }

        var swap = _states[$ code];

        if (_state == swap)
        {
            return;
        }

        if (!is_undefined(_state))
        {
            run_state(state_type.on_exit);
        }
        
        _state = swap;

        if (!is_undefined(_state))
        {
            run_state(state_type.on_enter);
        }
    }
	
	/// @description Gets the current state name.
	/// @returns {String} Returns the name of the current state.
	get_name = function()
	{
		return _state[$ "name"] ?? "";
	}

    /// @description Runs the action associated with the current state.
    /// @param {Enum.state_type} type The type of state action.
    run_state = function(type)
    {
        if (!is_undefined(type))
        {
            _state.action(type);
        }
    }
}
