/// @description Initialize default parameters.

/// @type {String}
/// @description The NATS host url.
#macro nats_host "ws://nats"

/// @type {Real}
/// @description The NATS port number.
#macro nats_port 9222

/// @inheritdoc
#macro event_publish obj_nats._protocol.publish

/// @inheritdoc
#macro event_subscribe obj_nats._protocol.subscribe

instance_singleton(obj_nats);

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

event_subscribe("user.logged_in", user_logged_in_event_handler);
event_subscribe("user.logged_out", user_logged_out_event_handler);
event_subscribe("chat.message", chat_message_event_handler);