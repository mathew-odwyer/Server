// TODO: Switch over to typescript.
// TODO: Ping and heartbeat (client side).
// TODO: Handle auth.login in authServer.js

// TODO: POST UserAccount/Login
// TODO: If failed to login, throw UnauthorizedError
// TODO: Otherwise, Store access token in redis with expiration.
// TODO: GET Player/Get
// TODO: Return player data

/*
    Example:

    return UserAccountClient.Login(dto)
        .then(response) => {
            storeAccessToken();
        }).catch((ex) => {
            throw new UnauthorizedError(ex.detail || ex.errors);
        }).next(() => {
            return PlayerClient.Get(); // will use redis to get access token for current user?
        });
*/

import { Server } from './servers/server.js';
import { registerAuthServer } from './servers/auth/authServer.js';
import { registerHealthServer } from './servers/health/healthServer.js';

const port = 8080;

const server = new Server({
    port: port,
});

registerAuthServer(server);
registerHealthServer(server);

console.log(`Server listening on port: '${port}'`);