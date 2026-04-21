import { Server } from "http";
import { logger } from "../../common/services/logging/logger.js";
import { HealthRpcController } from "../controllers/health/health-rpc-controller.js";
import { RpcServer } from "../../infrastructure/transport/jsonrpc/rpc-server.js";

export function registerRpcAdapter(server: Server)
{
    logger.info('Registering JSON-RPC 2.0 server...');

    const app = new RpcServer({
        server: server,
    });

    app.register(new HealthRpcController(app));
}
