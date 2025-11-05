/// @description Provides functions for managing the world (regions, interiors, etc).
/// @param {String} file_path The file path of the Tiled .world file.
function WorldManager(file_path = "winterhaven.world") constructor
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(WorldManager));
	
	/// @type {Real}
	/// @description The dimensions of a region (in pixels).
	static _region_size = 512;
	
	/// @type {Struct}
	/// @description The ID to region map.
	static _id_to_region_map = {};
	
	if (!file_exists(file_path))
	{
		throw new FileNotFoundError(file_path);
	}
	
	try
	{
		// Load the world maps.
		var buffer = buffer_load(file_path);
		var content = buffer_read(buffer, buffer_string);
		var world = json_parse(content);
		
		array_foreach(world.maps, function(map)
		{
			_logger.log(log_type.debug, $"Loading map: {map[$ "fileName"]}");
			_id_to_region_map[$ GetRegionId(map[$ "x"], map[$ "y"])] = map;
		});

		_logger.log(log_type.debug, $"Region IDs loaded: {struct_names_count(_id_to_region_map)}");
	}
	catch (ex)
	{
		_logger.log(log_type.error, $"Failed to parse world data: {ex}");
		throw new InvalidOperationError("Failed to parse world data.", ex);
	}
	
	/// @description Loads a region associated with the specified `region_id`.
	/// @param {String} region_id The region identifier.
	static LoadRegion = function(region_id)
	{
		var region = _id_to_region_map[$ region_id];
			
		// If we can't locate the region, or the region has been loaded, just return.
		if (is_undefined(region) || !is_undefined(region[$ "payload"]))
		{
			return;
		}
		
		// Extract the room asset from the file name.
		var file_path = region[$ "fileName"];
		var map_name = string_replace(filename_name(file_path), ".tmx", "");
		var room_index = asset_get_index(map_name);
		
		var x_coord = region[$ "x"];
		var y_coord = region[$ "y"];
		
		_logger.log(log_type.debug, $"Loading region: {map_name}");
		
		// Load the room instance associated with the region.
		region[$ "name"] = map_name;
		region[$ "payload"] = RoomLoader.Load(room_index, x_coord, y_coord);
		region[$ "loaded"] = true;
	}
	
	/// @description Loads a 3x3 chunk of regions based on the specified world coordinates.
	/// @param {Real} world_x The X world coordinate.
	/// @param {Real} world_y The Y world coordinate.
	static LoadChunk = function(world_x, world_y)
	{
		var coords = GetCoordinates(world_x, world_y);
		
		var offsets = [
			[-1, -1], [0, -1], [1, -1],
			[-1,  0], [0, 0], [1,  0],
			[-1,  1], [0,  1], [1,  1]
		];
		
		var current_chunk = [];
		var length = array_length(offsets);

		// Load any regions that should be in the chunk.
		for (var i = 0; i < length; i++)
		{
			var check_x = world_x + (offsets[i][0] * _region_size);
			var check_y = world_y + (offsets[i][1] * _region_size);
		
			var region_id = GetRegionId(check_x, check_y);
			array_push(current_chunk, region_id);
			
			LoadRegion(region_id);
		}
	}
	
	/// @description Gets region coordinates based on the specified world coordinates.
	/// @param {Real} world_x The X world coordinate.
	/// @param {Real} world_y The Y world coordinate.
	/// @returns {Struct} Returns the world coordinates converted to region coordinates.
	static GetCoordinates = function(world_x, world_y)
	{
		var region_size = _region_size;
		var rx = floor(world_x / region_size);
		var ry = floor(world_y / region_size);
		
		return { 
			x: rx,
			y: ry,
		};
	}
	
	/// @description Gets a region identifier based on the specified world coordinates.
	/// @param {Real} world_x The X world coordinate.
	/// @param {Real} world_y The Y world coordinate.
	/// @returns {String} Returns the region identifier based on the specified world coordinates.
	static GetRegionId = function(world_x, world_y)
	{
		var coords = GetCoordinates(world_x, world_y);
		return $"{coords.x}_{coords.y}";
	}
}

new WorldManager();
