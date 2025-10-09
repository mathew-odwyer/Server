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

register_health_rpc_server(_server);
register_auth_rpc_server(_server);