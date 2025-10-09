/// @description Handles an incoming login request from the client.
/// @param {Struct} params The username and password used to login.
/// @returns {String} Returns the user refresh token if the login was successful.
function rpc_handle_auth_login_request(params)
{
}

/// @description Handles an incoming registration request from the client.
/// @param {Struct} params The user credentials required for the registration.
/// @returns {Bool} Returns `true` if the user account was created; otherwise, `false`.
function rpc_handle_auth_register_request(params)
{
}

/// @description Register auth-related RPC handlers.
/// @param {Struct.Server} server The server.
function register_auth_rpc_server(server)
{
	server.register_handler("auth.login", rpc_handle_auth_login_request);
	server.register_handler("auth.register", rpc_handle_auth_register_request);
}