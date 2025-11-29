/// @description Represents a client connection to a server.
/// @param {Id.Socket} socket The underlying socket connection.
function ClientConnection(socket) constructor
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(ClientConnection));
	
	/// @type {Id.Socket}
	/// @description The underlying socket connection.
	_socket = socket;

	/// @type {Id.Buffer}
	/// @description The buffer used to send messages through the socket.
	_write_buffer = buffer_create(1, buffer_grow, 1);

	/// @type {Struct.AbortController}
	/// @description The abort controller used to signal an operation may have been aborted.
	_controller = new AbortController();

	/// @type {Bool}
	/// @description Indicates whether this instance is disposed.
	_is_disposed = false;
	
	/// @description Sends the specified `message` through the socket.
	/// @param {Struct} message The `message` to send through the socket.
	/// @returns {Bool} Returns `true` if the message was sent; otherwise, `false`.
	send = function(message)
	{
		if (_is_disposed || _controller.signal.get_aborted())
		{
			// Instead of throwing an `ObjectDisposedError we should simply just return false.
			// For more information on why this behaviour is intended, see #41
			return false;
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

		_controller.abort($"Socket disconnected with ID: '{_socket}'");
		
		if (_write_buffer != -1)
		{
			buffer_delete(_write_buffer);
			_write_buffer = -1;
		}
		
		if (_socket != -1)
		{
			_logger.log(log_type.debug, $"Socket disconnected with ID: '{_socket}'");
			
			network_destroy(_socket);
			_socket = -1;
		}
		
		_is_disposed = true;
	}

	/// @description Gets the abort controller signal for this `Struct.ClientConnection`.
	/// @returns {Struct.AbortSignal} Returns the abort controller signal for this `Struct.ClientConnection`.
	get_signal = function()
	{
		return _controller.signal;
	}
}
