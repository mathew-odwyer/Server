/// @description Loads a map into the world.
/// @param {String} map_data The map data (as a string).
/// @remarks The `map_load_map` function saves the map to a file on disk, then loads it using GMTiled2.
function map_load_map(map_data)
{
	/// @type {String}
	/// @description The map file name.
	static _file_name = "map.tmx";
	
	var file = file_text_open_write(_file_name);
	file_text_write_string(file, map_data);
	file_text_close(file);
	
	var data = tiled_read(_file_name);
	
	tiled_create(data);
	
	EventAggregator.Publish("MAP_LOADED", {
		map_cell_width: data[? "map_attribs"][? "width"],
		map_cell_height: data[? "map_attribs"][? "height"],
	});
	
	tiled_cleanup(data);
}
