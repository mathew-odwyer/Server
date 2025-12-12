/// @description Provides a NATS client-side protocol handler for a `Struct.Client`.
/// @param {Function} send The function used to send messages through the socket.
function NatsClientProtocol() constructor
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(NatsClientProtocol));

	/// @type {Function}
	/// @description The function used to send messages.
	_send = send;

	/// @description Handles an incoming `payload` from the server.
	/// @param {String} payload The payload sent to the client.
	handle_message = function(payload)
    {
        
    }
}
