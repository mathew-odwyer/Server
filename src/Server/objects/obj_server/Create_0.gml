/// @description Initialize default parameters.

/// @type {Real}
/// @description The server port number.
#macro server_port 8080

/// @type {Real}
/// @description The maximum number of client connections.
#macro server_max_clients 100

/// @type {Struct.Server}
/// @description The server.
_server = new Server(network_socket_tcp, server_port, server_max_clients);

_server.register_handler("health.ping", rpc_handle_health_ping_request);
_server.register_handler("health.heartbeat", rpc_handle_health_heartbeat_request);