import "reflect-metadata";
import { createServer } from 'http';
import { logger } from './common/services/logging/logger.js';
import { registerHttpAdapter } from './presentation/adapters/http-server.js';
import { registerRpcAdapter } from './presentation/adapters/rpc-server.js';

/*
    1. Create event-bus and nats-event-bus
    2. Ensure that subscribe and publish actually work
    3. Create cache and redis-cache
    4. Ensure get and set actually work
    5. Cleanup, documentation, prettify w/ VS Code extensions
    6. UserRpcController.register (push after) - remember controllers can take in dependencies that are services/interfaces and NOT HttpClients, etc.
    7. Ensure that errors are caught within the PRESENTATION layer by providing a event hook for rpc-server.ts (server.on("error", callback)).
        - This will allow us to convert HttpError's, or simply return a generic error to the client - OR lastly, return an RpcError
    7. DI Container. Then work on login, logout, refresh.
*/

const host = process.env["GATEWAY_HOST"] || '0.0.0.0';
const port = Number(process.env["GATEWAY_PORT"] ?? 8080);

const server = createServer();

registerHttpAdapter(server);
registerRpcAdapter(server);

server.listen(port, host, () => {
    logger.info(`Server listening on: '${host}:${port}'...`);
});
