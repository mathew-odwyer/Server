import express, { Router } from "express";
import { Server } from "http";
import { logger } from "../../common/services/logging/logger.js";

export function registerHttpAdapter(server: Server)
{
    logger.info('Registering HTTP server...');

    const app = express();
    const router = Router();

    server.on('request', app);

    router.get('/get', (_, response) => {
        response
            .status(200)
            .json({
                status: 'Healthiness',
                timestamp: new Date().toISOString(),
            });
    });

    app.use('/health', router);
}
