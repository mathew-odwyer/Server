/// @description Initialize default parameters.

instance_singleton(obj_ai_manager);

/// @type {Struct.Logger}
/// @description The logger.
_logger = new Logger(nameof(obj_ai_manager));

/// @type {Id.MpGrid}
/// @description The AI grid.
_grid = undefined;

generate_path = function(path, xfrom, yfrom, xto, yto)
{
	var xtile = floor(xto / cell_width) * cell_width;
	var ytile = floor(yto / cell_height) * cell_height;
	
	return mp_grid_path(_grid, path, xfrom, yfrom, xtile, ytile, true);
}

generate_random_path = function(path, xfrom, yfrom, xbegin, ybegin, radius)
{
	var xpos = irandom_range(xbegin - radius, xbegin + radius);
	var ypos = irandom_range(ybegin - radius, ybegin + radius);
	
	// Get the difference in position between where the object is and where it's going.
	var xdiff = abs(xfrom - xpos);
	var ydiff = abs(yfrom - ypos);
	
	// If the object wouldn't move very far, re-generate.
	if (xdiff < cell_width && ydiff < cell_height)
	{
		return generate_random_path(path, xfrom, yfrom, xbegin, ybegin, radius);
	}
	
	var result = generate_path(path, xfrom, yfrom, xpos, ypos);

	if (!result)
	{
		return generate_random_path(path, xfrom, yfrom, xbegin, ybegin, radius);
	}
	
	return result;
}

/// @description Sets up the path finding grid (registering all entities and collision tiles).
_setup_grid = function(data)
{
	_logger.log(log_type.debug, $"Setting up path finding grid for: '{room}'...");

	if (!is_undefined(_grid))
	{
		mp_grid_destroy(_grid);
	}
	
	_grid = mp_grid_create(0, 0, data.map_cell_width, data.map_cell_height, cell_width, cell_height);

	mp_grid_clear_all(_grid);
	mp_grid_add_instances(_grid, obj_entity_base, false);

	if (!layer_exists("Collisions"))
	{
	    return;
	}

	var collisions_layer = layer_get_id("Collisions");
	var tilemap_id = layer_tilemap_get_id(collisions_layer);

	if (tilemap_id == -1)
	{
	    return;
	}

	_logger.log(log_type.trace, "Adding collision tiles to path finding grid...");
	
	for (var i = 0; i < data.map_cell_width; i++)
	{
	    for (var j = 0; j < data.map_cell_height; j++)
	    {
	        var exists = tilemap_get(tilemap_id, i, j);
    
	        if (!exists)
	        {
	            continue;
	        }
    
			mp_grid_add_cell(_grid, i, j);
	    }
	}
}

EventAggregator.Subscribe(id, "MAP_LOADED", _setup_grid);

#region Debug View

/// @type {Bool}
/// @description Indicates whether the debug grid should be drawn.
_draw_grid = false;

/// @type {Real}
/// @description The alpha of the grid (if drawn).
_alpha = 0.2;

dbg_view("AI Manager", false);
dbg_section("Path Finding", true);
dbg_checkbox(ref_create(self, nameof(_draw_grid)), "Draw Grid");
dbg_slider(ref_create(self, nameof(_alpha)), 0.0, 1.0, "Alpha", 0.05);

#endregion
