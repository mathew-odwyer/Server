/// @description Initialize default parameters.

/// @type {String}
/// @description The Web API url.
#macro api_url "https://localhost:7256"

/// @type {Constant.SocketType}
/// @description The socket type.
#macro socket_type network_socket_ws

/// @type {Real}
/// @description The port number.
#macro server_port 8080

/// @type {Real}
/// @description The maximum number of connected clients.
#macro server_max_clients 200

/// @type {Struct.Server}
/// @description The server.
_server = new Server(socket_type, server_port, server_max_clients);

/// @type {Struct.JsonRpcServerProtocol}
/// @description The protocol to be used by the server.
_protocol = new JsonRpcServerProtocol(_server);

/// @inheritdoc
notify = _protocol.notify;

_protocol.register("health.ping", rpc_health_handle_ping_request);
_protocol.register("health.heartbeat", rpc_health_handle_heartbeat_request);

_protocol.register("user.register", rpc_user_handle_register_request);
_protocol.register("user.login", rpc_user_handle_login_request);
