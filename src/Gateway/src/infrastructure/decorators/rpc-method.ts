/**
 * @description Represents a unique key used to identify RPC Methods.
 * @type {symbol}
 */
export const RPC_METHODS_KEY = Symbol('rpc:methods');

/**
 * @description Represents a decorator function that is used to define a RPC Method.
 * @export
 * @param {?string} [name] The name of the function.
 * @returns {MethodDecorator} Returns the method decorator.
 */
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
