/// @description Handle network events.

var type = async_load[? "type"];
var socket = async_load[? "socket"];

switch (type)
{
	case network_type_connect:
		_server.add_connection(socket);
		break;
		
	case network_type_disconnect:
		user_logout(undefined, _server.get_connection(socket));
		_server.remove_connection(socket);
		break;
		
	case network_type_data:
		var buffer = async_load[? "buffer"];
		var identifier = async_load[? "id"];
		
		buffer_seek(buffer, buffer_seek_start, 0);
		var payload = buffer_read(buffer, buffer_text);
		
		_protocol.handle_message(identifier, payload);
		break;
}
