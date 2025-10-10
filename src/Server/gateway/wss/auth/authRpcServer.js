import { UserAccountClient } from '../../../common/clients/index.js';

/**
 * @description Handles an incoming login request from the client.
 * @param {import('../../../common/clients').LoginUserRequestDto} params The login user request data transfer object.
 * @returns {Promise<AxiosResponse<LoginUserResponseDto>>} A promise that resolves with the Axios response of the login request.
 */
async function handleLoginRequest(params)
{
    return await UserAccountClient.Login(params);
}

/**
 * @description Handles an incoming register user request from the client.
 * @param {import('../../../common/clients').RegisterUserRequestDto} params The register user request data transfer object.
 * @returns {Promise<AxiosResponse>} A promise that resolves with the Axios response of the register request.
 */
async function handleRegisterRequest(params)
{
    return await UserAccountClient.Register(params);
}

/**
 * @description Registers authentication-related RPC methods on the given server.
 * @param {import('../../services/rpc/rpcServer').RpcServer} server - The RPC server instance on which to register authentication methods.
 */
export function registerAuthRpcServer(server)
{
    server.registerHandler('auth.login', handleLoginRequest);
    server.registerHandler('auth.register', handleRegisterRequest);
}