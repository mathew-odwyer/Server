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
	/// @returns {Bool} Returns `true` if the message was sent; otherwise, `false.
	send = function(message)
	{
		buffer_seek(_write_buffer, buffer_seek_start, 0);
		buffer_write(_write_buffer, buffer_string, json_stringify(message));
		
		return network_send_raw(_socket, _write_buffer, buffer_tell(_write_buffer)) >= 0;
	}
	
	/// @description Cleanup resources.
	cleanup = function()
	{
		buffer_delete(_write_buffer);
		network_destroy(_socket);
	}
}
