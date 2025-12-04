function map_load_map(map_data)
{
	var file = file_text_open_write("map.tmx");
	file_text_write_string(file, map_data);
	file_text_close(file);
	
	tiled_oneshot("map.tmx");
}
