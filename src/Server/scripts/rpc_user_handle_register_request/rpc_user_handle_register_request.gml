/// @description Handles an incoming registration request from the client.
/// @param {Struct} credentials The credentials used to register a new account.
/// @returns {Struct.Promise} Returns a promise that is resolved when the API responds.
function rpc_user_handle_register_request(credentials)
{
    var user_account_client = new UserAccountClient({
        jsonrpc_error: true,
    });

    var dto = {
        username: credentials[$ "username"],
        password: credentials[$ "password"],
        email_address: credentials[$ "email_address"],
    };

    return user_account_client.register_async(dto);
}
