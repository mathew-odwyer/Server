/// @description Initialize default parameters.

#macro nats_host "ws://nats"

#macro nats_port 9222

/// @type {Struct.Logger}
/// @description The logger.
_logger = new Logger(nameof(obj_nats));

/// @type {Struct.Client}
/// @description The client.
_client = new Client(network_socket_ws);

/// @type {Struct.NatsClientProtocol}
/// @description The underlying protocol.
_protocol = new NatsClientProtocol(_client.send);

if (!_client.connect(nats_host, nats_port))
{
    throw new SocketError($"Failed to connect to NATS server: '{nats_host}:{nats_port}'")
}

_protocol.subscribe("UserLoggedInEvent", user_logged_in_event_handler);
_protocol.subscribe("UserLoggedOutEvent", user_logged_out_event_handler);
