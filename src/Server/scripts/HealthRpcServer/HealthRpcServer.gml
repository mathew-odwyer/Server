/// @description Provides health-related remote procedure calls.
function HealthRpcServer() constructor
{
	/// @description Registers the callbacks for the health-related RPCs.
	static RegisterCallbacks = function()
	{
		protocol.register("health.ping", _ping);
		protocol.register("health.heartbeat", _heartbeat);
	}
	
	/// @description Handles an incoming ping request.
	/// @param {Real} timestamp The timestamp.
	/// @returns {Real} Returns the timestamp back to the socket.
	static _ping = function(timestamp)
	{
		return timestamp;
	}
	
	/// @description Handles an incoming heartbeat request.
	/// @returns {Bool} Returns `true` to indicate the heartbeat was successful.
	static _heartbeat = function()
	{
		return true;
	}
}

new HealthRpcServer();