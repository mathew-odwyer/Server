export const RPC_METHODS_KEY = Symbol('rpc:methods');

export function RpcMethod(name?: string): MethodDecorator {
    return (target, propertyKey) => {
        const existing = Reflect.getMetadata(RPC_METHODS_KEY, target.constructor) || [];

        existing.push({
            methodName: propertyKey,
            rpcName: name ?? propertyKey,
        });

        Reflect.defineMetadata(RPC_METHODS_KEY, existing, target.constructor);
    };
}
