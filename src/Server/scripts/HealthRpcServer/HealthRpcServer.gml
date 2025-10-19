/// @description Handles an incoming ping request.
/// @param {Real} timestamp The timestamp sent by the client.
/// @returns {Real} Returns the timestamp back to the client.
function rpc_health_handle_ping_request(timestamp)
{
    return timestamp;
}

/// @description Handles an incoming heartbeat request.
/// @returns {Bool} Returns `true`, indicating the server is responsive.
function rpc_health_handle_heartbeat_request()
{
    return true;
}

/// @self Id.Instance.obj_server
/// @description Registers the health-related RPC callbacks.
function rpc_health_register_callbacks()
{
    _protocol.register("health.ping", rpc_health_handle_ping_request);
    _protocol.register("health.heartbeat", rpc_health_handle_heartbeat_request);
}
