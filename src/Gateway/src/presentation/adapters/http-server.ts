import express, { Router } from 'express';
import { Server } from 'http';
import { logger } from '../../common/services/logging/logger.js';

/**
 * @description Registers the HTTP service adapter.
 * @export
 * @param {Server} server The server instance to connect to the adapter.
 */
export function registerHttpAdapter(server: Server) {
    logger.info('Registering HTTP server...');

    const app = express();
    const router = Router();

    server.on('request', app);

    router.get('/get', (_, response) => {
        response.status(200).json({
            status: 'Healthiness',
            timestamp: new Date().toISOString(),
        });
    });

    app.use('/health', router);
}
