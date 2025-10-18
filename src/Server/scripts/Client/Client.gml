/// @description Represents a network client.
function Client() constructor
{
	/// @type {Id.Socket}
	/// @description The underlying network socket.
	_socket = network_create_socket(network_socket_ws);

	/// @type {Id.Buffer}
	/// @description The buffer used to send messages through the socket.
	_write_buffer = buffer_create(1, buffer_grow, 1);
	
	if (_socket < 0)
	{
		throw new SocketError("Failed to create network socket.");
	}
	
	/// @description Connects the client to the specified `host` and `port`.
	/// @param {String} host The host name or URL.
	/// @param {Real} port The port number.
	/// @returns {Bool} Returns `true` if the client connected; otherwise, `false`.
	connect = function(host, port)
	{
		return network_connect_raw(_socket, host, port) >= 0;
	}
	
	/// @description Sends the specified `message` through the socket.
	/// @param {Struct} message - The `message` to send through the socket.
	send = function(message)
	{
		buffer_seek(_write_buffer, buffer_seek_start, 0);
		buffer_write(_write_buffer, buffer_string, json_stringify(message));
		
		return network_send_packet(_socket, _write_buffer, buffer_tell(_write_buffer));
	}
	
	/// @description Cleanup resources.
	cleanup = function()
	{
		buffer_delete(_write_buffer);
		network_destroy(_socket);
	}
}
