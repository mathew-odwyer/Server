/// @description Represents a client connection to a server.
/// @param {Id.Socket} socket The underlying socket connection.
function ClientConnection(socket) constructor
{
	/// @type {Id.Socket}
	/// @description The underlying socket connection.
	_socket = socket;

	/// @type {Id.Buffer}
	/// @description The buffer used to send messages through the socket.
	_write_buffer = buffer_create(1, buffer_grow, 1);
	
	/// @description Sends the specified `message` through the socket.
	/// @param {Struct} message The `message` to send through the socket.
	send = function(message)
	{
		buffer_seek(_write_buffer, buffer_seek_start, 0);
		buffer_write(_write_buffer, buffer_string, json_stringify(message));
		network_send_packet(_socket, _write_buffer, buffer_tell(_write_buffer));
	}
	
	/// @description Disconnects the client from the server.
	disconnect = function()
	{
		buffer_delete(_write_buffer);
		network_destroy(_socket);
	}
}