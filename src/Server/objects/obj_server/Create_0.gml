/// @description Initialize default parameters.

/// @type {String}
/// @description The Web API URL.
#macro api_url "https://gateway.mantanimus.com.au"

/// @type {Real}
/// @description The server port number.
#macro server_port 6512

/// @type {Real}
/// @description The maximum number of client connections.
#macro server_max_clients 100

/// @type {Struct.JsonRpcServerProtocol}
/// @description The server protocol.
#macro protocol obj_server._protocol

/// @type {Struct.Server}
/// @description The server.
_server = new Server(server_port, server_max_clients);

/// @type {Struct.JsonRpcServerProtocol}
/// @description The protocol to be used by the server.
_protocol = new JsonRpcServerProtocol(_server);

HealthRpcServer.RegisterCallbacks();