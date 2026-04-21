// @ts-nocheck

import { Server } from 'rpc-websockets';
import type { ServerOptions } from 'rpc-websockets';
import { RPC_COTNROLLER_KEY } from '../../decorators/rpc-controller';
import { RPC_METHODS_KEY } from '../../decorators/rpc-method';
import { logger } from '../../../common/services/logging/logger';

type RpcMethod = { methodName: string | symbol; rpcName: string };

export type ClientConnection = {
    id: string;
}

export type ServerContext = {
    client: ClientConnection;
}

export class RpcControllerBase {
    protected server: RpcServer;

    constructor(server: RpcServer) {
        this.server = server;
    }
};

export class RpcServer {
    private readonly server: Server;

    constructor(options: ServerOptions) {
        this.server = new Server(options);

        this.server.on('connection', (socket, req) => {
            logger.info(`Client Connected: ${req.socket.remoteAddress}`);

            socket.on('close', (code, reason) => {
                logger.info(`Client Disconnected: ${req.socket.remoteAddress} - Code: ${code}, Reason: ${reason}`);
            });
        });

        this.server.on('error', (error) => {
            logger.error('RPC Server error:', error);
        });
    }

    register(controller: RpcControllerBase) {
        const constructor = controller.constructor;
        const prefix: string = Reflect.getMetadata(RPC_COTNROLLER_KEY, constructor) || '';
        const methods: RpcMethod[] = Reflect.getMetadata(RPC_METHODS_KEY, constructor) ?? [];

        logger.info(`Registering RPC Controller: '${prefix}'...`);

        for (const { methodName, rpcName } of methods) {
            const fullName = (prefix ? `${prefix}.${rpcName}` : rpcName).toLowerCase();
            
            logger.debug(`Registering RPC: '${fullName}'...`);

            this.server.register(fullName, async (params:unknown, socketId:string) => {
                const context: ServerContext = {
                    client: this.getClient(socketId),
                };

                return await controller[methodName](params, context);
            });
        }
    }

    getClient(socketId:string): ClientConnection {
        const namespace = this.server.of('/');
        const sockets:Record<string, any> = namespace.connected();
        const socket = sockets[socketId];

        return {
            id: socket.id,
        };
    }
}
