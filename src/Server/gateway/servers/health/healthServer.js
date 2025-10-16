/**
 * @description Handles an incoming ping request from the client.
 * @param {number} ping - The ping value (in milliseconds) sent by the client.
 * @returns {number} The same ping value received, echoing it back to the client.
 */
function handlePingRequest(ping)
{
    return ping;
}

/**
 * @description Handles an incoming heartbeat request from the client.
 * @returns {boolean} Always returns true to indicate the server is alive.
 */
function handleHeartbeatRequest()
{
    return true;
}

/**
 * @description Registers health-related RPC methods on the given server.
 * @param {import('../server').Server} server - The RPC server instance on which to register health methods.
 */
export function registerHealthServer(server)
{
    server.registerHandler('health.ping', handlePingRequest);
    server.registerHandler('health.heartbeat', handleHeartbeatRequest);
}