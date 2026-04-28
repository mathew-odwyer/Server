import { RpcController } from '../../../infrastructure/decorators/rpc-controller.js';
import { RpcMethod } from '../../../infrastructure/decorators/rpc-method.js';
import { RpcControllerBase } from '../../../infrastructure/transport/jsonrpc/rpc-server.js';

/**
 * @description Represents a controller that defines health-related functions.
 * @export
 * @class HealthRpcController
 * @typedef {HealthRpcController}
 * @extends {RpcControllerBase}
 */
@RpcController('Health')
export class HealthRpcController extends RpcControllerBase {
    /**
     * @description Handles a ping request.
     * @param {number} timestamp The timestamp to be echoed back to the client.
     * @returns {number} Returns the `timestamp` back to the client.
     */
    @RpcMethod('ping')
    ping(timestamp: number): number {
        return timestamp;
    }

    /**
     * @description Handles a heartbeat request.
     * @returns {boolean} Returns `true`, indicating the server is healthy.
     */
    @RpcMethod('heartbeat')
    heartbeat(): boolean {
        return true;
    }
}
