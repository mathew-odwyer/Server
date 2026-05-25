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
	
	/// @type {Bool}
	/// @description Indicates whether this instance is disposed.
	_is_disposed = false;
	
	if (_socket < 0)
	{
		throw new SocketError("Failed to create network socket.");
	}
	
	/// @description Connects to the server asynchronously.
	/// @param {String} host The host name or URL.
	/// @param {Real} port The port number.
	/// @returns {Bool} Returns `true` if the client connected; otherwise, `false`.
	connect = function(host, port)
	{
		if (_is_disposed)
		{
			throw new ObjectDisposedError(nameof(Client));
		}
		
		_logger.log(log_type.debug, $"Connecting to server: '{host}:{port}'...");
		return network_connect_raw(_socket, host, port) >= 0;
	}
	
	/// @description Sends the specified `message` through the socket.
	/// @param {Struct} message - The `message` to send through the socket.
	/// @returns {Bool} Returns `true` if the message was sent; otherwise, `false`.
	send = function(message)
	{
		if (_is_disposed)
		{
			throw new ObjectDisposedError(nameof(Client));
		}
		
		buffer_seek(_write_buffer, buffer_seek_start, 0);
		buffer_write(_write_buffer, buffer_string, json_stringify(message));
		
		return network_send_raw(_socket, _write_buffer, buffer_tell(_write_buffer)) >= 0;
	}
	
	/// @description Cleanup resources.
	cleanup = function()
	{
		if (_is_disposed)
		{
			return;
		}
		
		if (_write_buffer != -1)
		{
			buffer_delete(_write_buffer);
			_write_buffer = -1;
		}
		
		if (_socket != -1)
		{
			network_destroy(_socket);
			_socket = -1;
		}
		
		_is_disposed = true;
	}
}
