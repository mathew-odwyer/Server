import { RpcController } from "../../../infrastructure/decorators/rpc-controller.js";
import { RpcMethod } from "../../../infrastructure/decorators/rpc-method.js";
import { RpcControllerBase } from "../../../infrastructure/transport/jsonrpc/rpc-server.js";

@RpcController('Health')
export class HealthRpcController extends RpcControllerBase
{
    @RpcMethod('ping')
    ping(timestamp: number): number
    {
        return timestamp;
    }

    @RpcMethod('heartbeat')
    heartbeat(): boolean
    {
        return true;
    }
}
