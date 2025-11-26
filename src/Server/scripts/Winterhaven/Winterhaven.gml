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

/// @type {Real}
/// @description The distance between two instances that must be met before an action can be executed.
#macro action_radius 2 * cell_width
