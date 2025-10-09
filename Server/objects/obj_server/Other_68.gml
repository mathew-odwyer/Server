/// @description Handle incoming connections, disconnections, requests and notifications.

var type = async_load[? "type"];

switch (type)
{
    case network_type_connect:
        _server.add_connection(async_load[? "socket"]);
        break;

    case network_type_disconnect:
        _server.remove_connection(async_load[? "socket"]);
        break;

    case network_type_data:
        var socket = async_load[? "id"];
        var read_buffer = async_load[? "buffer"];
		
        buffer_seek(read_buffer, buffer_seek_start, 0);
        var json = buffer_read(read_buffer, buffer_string);
		
        _server.handle_message(socket, json);
        break;
}