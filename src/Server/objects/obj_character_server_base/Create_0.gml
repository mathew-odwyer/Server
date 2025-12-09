/// @description Initialize default parameters.

event_inherited();

/// @type {Array}
/// @description The actions performed this tick.
_actions_performed = [];

/// @type {Function}
/// @description The callback used to process all actions for the current tick.
_process_tick = function() {};

alarm[0] = tick_rate;
