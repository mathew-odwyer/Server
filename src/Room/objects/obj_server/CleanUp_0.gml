/// @description Cleanup resources.

delete _map_client;
delete _protocol;

_server.cleanup();
delete _server;