/// @description Loads a map into the world.
/// @param {String} map_data The map data (as a string).
/// @remarks The `map_load_map` function saves the map to a file on disk, then loads it using GMTiled2.
function map_load_map(map_data)
{
	var file = file_text_open_write("map.tmx");
	file_text_write_string(file, map_data);
	file_text_close(file);
	
	tiled_oneshot("map.tmx");
}