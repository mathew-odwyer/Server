/// @description Handle network events.

var type = async_load[? "type"];

switch (type)
{
	case network_type_connect:
	case network_type_non_blocking_connect:
		var connection = _server.add_connection(async_load[? "socket"]);
		break;
	
	case network_type_disconnect:
		_server.remove_connection(async_load[? "socket"]);
		break;
	
	case network_type_data:
		var socket = async_load[? "id"];
		var buffer = async_load[? "buffer"];
		
		buffer_seek(buffer, buffer_seek_start, 0);
		var payload = buffer_read(buffer, buffer_string);
		
		_protocol.handle_message(socket, payload);
		break;
}