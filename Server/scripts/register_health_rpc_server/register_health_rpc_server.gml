/// @description Handles an incoming ping request from the client.
/// @param {Real} timestamp The timestamp sent by the client.
/// @returns {Real} Returns the timestamp sent by the client.
function rpc_handle_health_ping_request(timestamp)
{
	return timestamp;
}

/// @description handles an incoming heartbeat request by the client.
/// @returns {Bool} Returns `true`.
function rpc_handle_health_heartbeat_request()
{
	// TODO: Validate JWT here if the user has logged in.
	return true;
}

/// @description Register health-related RPC handlers.
/// @param {Struct.Server} server The server.
function register_health_rpc_server(server)
{
	server.register_handler("health.ping", rpc_handle_health_ping_request);
	server.register_handler("health.heartbeat", rpc_handle_health_heartbeat_request);
}