/// @description Initialize default parameters.

instance_singleton(obj_server);

/// @type {Real}
/// @description The server port number.
#macro server_port 8080

/// @type {Real}
/// @description The maximum number of client connections.
#macro server_max_clients 100

/// @type {Struct.Server}
/// @description The server.
_server = new Server(server_port, server_max_clients);

/// @type {Struct.JsonRpcServerProtocol}
/// @description The protocol to be used by the server.
_protocol = new JsonRpcServerProtocol(_server);
