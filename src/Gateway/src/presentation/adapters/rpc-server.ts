import { Server } from 'http';
import { logger } from '../../common/services/logging/logger.js';
import { HealthRpcController } from '../controllers/health/health-rpc-controller.js';
import { RpcServer } from '../../infrastructure/transport/jsonrpc/rpc-server.js';

/**
 * @description Registers the JSON-RPC 2.0 WebSocket adapter.
 * @export
 * @param {Server} server The server to connect to the adapter.
 */
export function registerRpcAdapter(server: Server) {
    logger.info('Registering JSON-RPC 2.0 server...');

    const app = new RpcServer({
        server: server,
    });

    app.register(new HealthRpcController(app));
}
