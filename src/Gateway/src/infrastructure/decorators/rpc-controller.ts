/**
 * @description Represents a unique key used to identify RPC Controllers.
 * @type {symbol}
 */
export const RPC_COTNROLLER_KEY = Symbol('rpc:controller');

/**
 * @description Represents a decorator function that is used to define a RPC Controller.
 * @export
 * @param {string} prefix The prefix of the controller.
 * @returns {ClassDecorator} Returns the class decorator.
 */
export function RpcController(prefix: string): ClassDecorator {
    return (target) => {
        Reflect.defineMetadata(RPC_COTNROLLER_KEY, prefix, target);
    };
}
