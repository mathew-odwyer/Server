/// @description Handle network events.

var type = async_load[? "type"];

switch (type)
{
	case network_type_data:
		var buffer = async_load[? "buffer"];
		
		buffer_seek(buffer, buffer_seek_start, 0);
		var payload = buffer_read(buffer, buffer_text);
		
		_protocol.handle_message(payload);
		break;
}
