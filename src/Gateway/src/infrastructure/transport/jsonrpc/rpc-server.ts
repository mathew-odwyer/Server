// @ts-nocheck

import { Server } from 'rpc-websockets';
import type { ServerOptions } from 'rpc-websockets';
import { RPC_COTNROLLER_KEY } from '../../decorators/rpc-controller';
import { RPC_METHODS_KEY } from '../../decorators/rpc-method';
import { logger } from '../../../common/services/logging/logger';

/**
 * @description Defines a type that represents an RPC Method.
 * @typedef {RpcMethod}
 */
type RpcMethod = { methodName: string | symbol; rpcName: string };

/**
 * @description Defines a type that represents a client connection.
 * @export
 * @typedef {ClientConnection}
 */
export type ClientConnection = {
    id: string;
};

/**
 * @description Defines a type that provides properties related to the context of the client.
 * @export
 * @typedef {ServerContext}
 */
export type ServerContext = {
    client: ClientConnection;
};

/**
 * @description Represents a base class for an RPC Controller.
 * @export
 * @class RpcControllerBase
 * @typedef {RpcControllerBase}
 */
export class RpcControllerBase {
    /**
     * @description Represents the RPC server that is bound to this controller.
     * @protected
     * @readonly
     * @type {RpcServer}
     */
    protected readonly server: RpcServer;

    /**
     * Creates an instance of `RpcControllerBase`.
     * @constructor
     * @param {RpcServer} server The RPC server that is to be bound to this controller.
     */
    constructor(server: RpcServer) {
        this.server = server;
    }
}

/**
 * @description Represents a JSON-RPC 2.0 WebSocket server.
 *
 * @export
 * @class RpcServer
 * @typedef {RpcServer}
 */
export class RpcServer {
    /**
     * @description Represents the underlying rpc-websockets server instance.
     * @private
     * @readonly
     * @type {Server}
     */
    private readonly server: Server;

    /**
     * Creates an instance of `RpcServer`.
     * @constructor
     * @param {ServerOptions} options The server options used when creating the underlying server instance.
     */
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

    /**
     * @description Registers a RPC controller and binds it methods.
     * @param {RpcControllerBase} controller The controller to be registered.
     */
    register(controller: RpcControllerBase) {
        const constructor = controller.constructor;
        const prefix: string = Reflect.getMetadata(RPC_COTNROLLER_KEY, constructor) || '';
        const methods: RpcMethod[] = Reflect.getMetadata(RPC_METHODS_KEY, constructor) ?? [];

        logger.info(`Registering RPC Controller: '${prefix}'...`);

        for (const { methodName, rpcName } of methods) {
            const fullName = (prefix ? `${prefix}.${rpcName}` : rpcName).toLowerCase();

            logger.debug(`Registering RPC: '${fullName}'...`);

            this.server.register(fullName, async (params: unknown, socketId: string) => {
                const context: ServerContext = {
                    client: this.getClient(socketId),
                };

                return await controller[methodName](params, context);
            });
        }
    }

    /**
     * @description Fetches a client connection based on the specified `socketId`.
     * @param {string} socketId The socket identifier used to locate the client connection.
     * @returns {ClientConnection} Returns the client connection associated with the specified `socketId`.
     */
    getClient(socketId: string): ClientConnection {
        const namespace = this.server.of('/');
        const sockets: Record<string, any> = namespace.connected();
        const socket = sockets[socketId];

        return {
            id: socket.id,
        };
    }
}
