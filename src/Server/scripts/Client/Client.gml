/// @description Represents a network client.
/// @param {Constant.SocketType} type The socket type.
function Client(type) constructor
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(Client));
	
	/// @type {Id.Socket}
	/// @description The underlying network socket.
	_socket = network_create_socket(type);

	/// @type {Id.Buffer}
	/// @description The buffer used to send messages through the socket.
	_write_buffer = buffer_create(1, buffer_grow, 1);
	
	if (_socket < 0)
	{
		throw "Failed to create network socket.";
	}
	
	/// @description Connects to the server asynchronously.
	/// @param {String} host The host name or URL.
	/// @param {Real} port The port number.
	connect = function(host, port)
	{
		_logger.log(log_type.debug, $"Connecting to server: '{host}:{port}'...");
		return network_connect_raw(_socket, host, port) >= 0;
	}
	
	/// @description Sends the specified `message` through the socket.
	/// @param {Struct} message - The `message` to send through the socket.
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
