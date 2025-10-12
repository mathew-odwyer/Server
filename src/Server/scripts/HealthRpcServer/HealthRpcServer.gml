/// @description Provides health-related remote procedure calls.
function HealthRpcServer() constructor
{
	/// @description Registers the callbacks for the health-related RPCs.
	static RegisterCallbacks = function()
	{
		protocol.register("health.ping", Ping);
		protocol.register("health.heartbeat", Heartbeat);
	}
	
	/// @description Handles an incoming ping request.
	/// @param {Real} timestamp The timestamp.
	/// @returns {Real} Returns the timestamp back to the socket.
	static Ping = function(timestamp)
	{
		return timestamp;
	}
	
	/// @description Handles an incoming heartbeat request.
	/// @returns {Bool} Returns `true` to indicate the heartbeat was successful.
	static Heartbeat = function()
	{
		return true;
	}
}

new HealthRpcServer();