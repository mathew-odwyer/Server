/// @description Handles an incoming ping request from the client.
/// @param {Real} timestamp The timestamp sent by the client.
/// @returns {Real} Returns the `timestamp`; echoing it back to the client.
function rpc_health_handle_ping_request(timestamp)
{
	return timestamp;
}
