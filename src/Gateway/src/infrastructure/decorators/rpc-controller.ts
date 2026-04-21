export const RPC_COTNROLLER_KEY = Symbol('rpc:controller');

export function RpcController(prefix: string): ClassDecorator {
    return (target) => {
        Reflect.defineMetadata(RPC_COTNROLLER_KEY, prefix, target);
    };
}
