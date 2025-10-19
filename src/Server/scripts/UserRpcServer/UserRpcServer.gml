/// @description Handles an incoming user registration request.
/// @param {Struct} credentials The user credentials.
/// @returns {Struct.Promise} A promise that resolves when registration is complete.
function rpc_user_handle_register_request(credentials)
{
    if (!is_struct(credentials) ||
        !struct_exists(credentials, "username")||
        !struct_exists(credentials, "password") ||
        !struct_exists(credentials, "email_address"))
    {
        // This should never happen if the client is well-behaved.
        // However, we validate input to avoid potential issues.
        throw new ValidationError("You must provide a username, password, and email address to register.");
    }

    Logger.Log(log_type.information, $"Handling user registration for '{credentials.username}'");

    var dto = {
        username: credentials.username,
        password: credentials.password,
        email_address: credentials.email_address
    };

    return UserAccountClient.RegisterAsync(dto)
        .next(method({dto}, function()
        {
			Logger.Log(log_type.information, $"User registered with username: {dto.username}");
            return true;
        }))
        .fail(function(error)
        {
			var detail = error.data[$ "detail"] ?? "An unknown error occurred during registration";
			Logger.Log(log_type.information, $"Failed to register user: {detail}");
			
            throw new ValidationError(detail);
        });
}

/// @self Id.Instance.obj_server
/// @description Registers the user-related RPC callbacks.
function rpc_user_register_callbacks()
{
    _protocol.register("user.register", rpc_user_handle_register_request);
}