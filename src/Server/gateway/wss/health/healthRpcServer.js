/**
 * @description Handles an incoming ping request from the client.
 * @param {number} timestamp - The timestamp value (in milliseconds) sent by the client.
 * @returns {number} The same timestamp value received, echoing it back to the client.
 */
function handlePingRequest(timestamp)
{
    return timestamp;
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
 * @param {import('../../services/rpc/rpcServer').RpcServer} server - The RPC server instance on which to register health methods.
 */
export function registerHealthRpcServer(server)
{
    server.registerHandler('health.ping', handlePingRequest);
    server.registerHandler('health.heartbeat', handleHeartbeatRequest);
}