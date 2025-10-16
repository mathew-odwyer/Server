import { Server as RpcServer } from 'rpc-websockets';
import camelcaseKeys from "camelcase-keys";

/**
 * @export
 * @class Server
 * @description Provides a WSS server that uses the JSON-RPC 2.0 protocol.
 */
export class Server
{
    /**
     * Creates an instance of Server.
     * @param {import('ws').WebSocket.ServerOptions} options - The server options.
     * @memberof Server
     */
    constructor(options)
    {
        this.server = new RpcServer(options);
        
        this.server.on('connection', (socket, request) => {
            console.info('Client Connected:', request.socket.remoteAddress);

            socket.on('close', async () => {
                console.info('Client Disconnected:', request.socket.remoteAddress);

                // TODO: if (socket.user) { ... }
            })
        });

        this.server.on('error', (error) => {
            console.error('Server error:', error);
        });
    }

    /**
     * @description Registers a JSON-RPC 2.0 request handler.
     * @param {string} method - The procedure to be invoked by the server.
     * @param {Function} callback - The callback that is executed when invoking the procedure.
     * @return {*} The instanced that can be used to protect the procedure. 
     * @memberof Server
     */
    registerHandler(method, callback)
    {
        console.info('Registering JSON-RPC handler:', method);

        return this.server.register(method, async (params, socketId) => {
            const namespace = this.server.of('/');
            const clientMap = namespace.clients().clients;
            const client = clientMap.get(socketId);

            const context = {
                server: this,
                client: client,
                socketId: socketId,
            };

            return await callback.call(context, camelcaseKeys(params, { deep: true }));
        });
    }

    /**
     * @description Sends a JSON-RPC 2.0 notification to the specified `client`.
     * @param {number} socketId - The socket ID to send the notification to.
     * @param {string} method - The procedure to be invoked by the client.
     * @param {Array} params - The parameters of the procedure usable by the client.
     * @memberof Server
     */
    sendNotification(socketId, method, params)
    {
        const payload = JSON.stringify({
            jsonrpc: '2.0',
            method:  method,
            params:  params,
        });

        const namespace = this.server.of('/');
        const clientMap = namespace.clients().clients;
        const client = clientMap.get(socketId);

        client.send(payload);
    }
}
