import { RpcServer } from "../services/rpc/rpcServer.js";
import { registerHealthRpcServer } from "./health/healthRpcServer.js";
import { registerAuthRpcServer } from "./auth/authRpcServer.js";

/**
 * @description Starts the WSS server.
 * @param {import('http').Server} app
 */
export function startWssServer(app)
{
    const server = new RpcServer({
        server: app,
    });

    registerHealthRpcServer(server);
    registerAuthRpcServer(server);
}