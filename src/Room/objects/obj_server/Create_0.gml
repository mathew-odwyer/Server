/// @description Initialize default parameters.

/// @type {String}
/// @description The Web API url.
#macro api_url environment_get_variable("API_URL")

/// @type {String}
/// @description The API key.
#macro api_key environment_get_variable("API_KEY")

/// @type {Constant.SocketType}
/// @description The socket type.
#macro socket_type network_socket_ws

/// @type {Real}
/// @description The port number.
#macro server_port 8080

/// @type {Real}
/// @description The maximum number of connected clients.
#macro server_max_clients 200

instance_singleton(obj_server);

/// @type {String}
/// @description The map data.
map_data = undefined;

/// @type {Struct.Server}
/// @description The server.
_server = new Server(socket_type, server_port, server_max_clients);

/// @type {Struct.JsonRpcServerProtocol}
/// @description The protocol to be used by the server.
_protocol = new JsonRpcServerProtocol(_server);

/// @inheritdoc
notify = _protocol.notify;

_protocol.register("health.ping", health_ping);
_protocol.register("health.heartbeat", health_heartbeat);

_protocol.register("user.register", user_register);
_protocol.register("user.login", user_login);
_protocol.register("user.refresh", user_refresh);
_protocol.register("user.logout", user_logout);

_protocol.register("player.action", player_action);

_protocol.register("chat.send_message", chat_send_message);
