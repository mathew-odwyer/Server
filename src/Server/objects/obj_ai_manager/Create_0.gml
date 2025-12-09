/// @description Initialize default parameters.

instance_singleton(obj_ai_manager);

/// @type {Struct.Logger}
/// @description The logger.
_logger = new Logger(nameof(obj_ai_manager));

/// @type {Id.MpGrid}
/// @description The AI grid.
_grid = undefined;

/// @type {Real}
/// @description The A* path-finding resolution.
_resolution = 2;

/// @description Attempts to generate a random path (retrying 10 times).
/// @param {Asset.GMPath} path The path object used for generation.
/// @param {Real} xfrom The X-coordinate of the object.
/// @param {Real} yfrom The Y-coordinate of the object.
/// @param {Real} xbegin The starting X-coordinate of the object.
/// @param {Real} ybegin The starting Y-coordinate of the object.
/// @param {Real} radius The path radius.
/// @returns {Bool} Returns `true` if the path was generated successfully; otherwise, `false`.
try_generate_random_path = function(path, xfrom, yfrom, xbegin, ybegin, radius)
{
	static _max_retries = 10;
	
	for (var i = 0; i < _max_retries; i++)
	{
		var xpos = irandom_range(xbegin - radius, xbegin + radius);
		var ypos = irandom_range(ybegin - radius, ybegin + radius);
	
		if (mp_grid_path(_grid, path, xfrom, yfrom, xpos, ypos, true))
		{
			return true;
		}
	}
	
	return false;
}

/// @description Sets up the path finding grid (registering all entities and collision tiles).
_setup_grid = function(data)
{
	_logger.log(log_type.debug, $"Setting up path finding grid for: '{room}'...");

	if (!is_undefined(_grid))
	{
		mp_grid_destroy(_grid);
	}
	
	var grid_width = data.map_cell_width  * _resolution;
    var grid_height = data.map_cell_height * _resolution;

    var sub_cell_width = cell_width / _resolution;
    var sub_cell_height = cell_height / _resolution;
	
	_logger.log(log_type.debug, $"Setting up path grid: '{grid_width}x{grid_height}' with cell '{sub_cell_width}x{sub_cell_height}'");
	
	_grid = mp_grid_create(0, 0, grid_width, grid_height, sub_cell_width, sub_cell_height);

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

            // mark all subdivided cells as blocked.
            var base_x = i * _resolution;
            var base_y = j * _resolution;

            for (var sx = 0; sx < _resolution; sx++)
			{
	            for (var sy = 0; sy < _resolution; sy++)
		        {
			        mp_grid_add_cell(_grid, base_x + sx, base_y + sy);
				}
			}
		}
	}
}

EventAggregator.Subscribe(id, "MAP_LOADED", _setup_grid);
