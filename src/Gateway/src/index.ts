import 'reflect-metadata';
import { createServer } from 'http';
import { createClient, RedisClientType } from 'redis';
import { connect } from 'nats.ws';
import { logger } from './common/services/logging/logger.js';
import { registerHttpAdapter } from './presentation/adapters/http-server.js';
import { registerRpcAdapter } from './presentation/adapters/rpc-server.js';
import { NatsEventsBus } from './infrastructure/services/events/nats-event-bus.js';
import { EventBus } from './application/adapters/events/event-bus.js';
import { Cache } from './application/adapters/data/cache.js';
import { RedisCache } from './infrastructure/services/data/redis-cache.js';

/*
    1. Create event-bus and nats-event-bus :)
    2. Ensure that subscribe and publish actually work :)
    3. Create cache and redis-cache :)
    4. Ensure get and set actually work :)
    5. Cleanup, documentation, prettify w/ VS Code extensions
    6. UserRpcController.register (push after) - remember controllers can take in dependencies that are services/interfaces and NOT HttpClients, etc.
    7. Ensure that errors are caught within the PRESENTATION layer by providing a event hook for rpc-server.ts (server.on("error", callback)). 
        - Also what about try/catch in the this.server.register - that is better.
        - This will allow us to convert HttpError's, or simply return a generic error to the client - OR lastly, return an RpcError
    7. DI Container. Then work on login, logout, refresh.
*/

type DataTest = {
    test: number;
};

(async () => {
    const host = process.env['GATEWAY_HOST'] || '0.0.0.0';
    const port = Number(process.env['GATEWAY_PORT'] ?? 8080);

    const server = createServer();

    registerHttpAdapter(server);
    registerRpcAdapter(server);

    const nats = await connect({
        servers: process.env['NATS_URL'] || 'ws://nats:9222',
    });

    const eventBus: EventBus = new NatsEventsBus(nats);

    const redis: RedisClientType = (await createClient({
        url: process.env['REDIS_URL'] || '',
    }).connect()) as RedisClientType;

    const cache: Cache = new RedisCache(redis);

    eventBus.subscribe('test.message', async (data: DataTest) => {
        console.log(data.test);

        await cache.set('test', data.test, {
            ttl: 5,
        });

        const context = {
            cache,
        };

        setTimeout(async () => {
            const result = await context.cache.get<DataTest>('test');

            if (result.exists) {
                console.log(result.value);
            }
        }, 2500);

        setTimeout(async () => {
            const result = await context.cache.get<DataTest>('test');

            if (result.exists) {
                console.error('WRONG');
                return;
            }

            console.log(result);
        }, 6000);
    });

    eventBus.publish('test.message', {
        test: 100,
    });

    server.listen(port, host, () => {
        logger.info(`Server listening on: '${host}:${port}'...`);
    });
})();
