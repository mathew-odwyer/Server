/// @descriptions Registers a new user account.
/// @param {Struct} credentials The user account credentials.
/// @param {Struct.ClientConnection} connection The client connection that sent the request.
/// @returns {Struct.__Promise} Returns a promise that is resolved when the API responds.
function user_register(credentials, connection)
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(user_register));

	if (!is_struct(credentials))
	{
		return;
	}
	
	var user_account_client = new UserAccountClient({
		signal: connection.get_signal(),
		jsonrpc_error: true,
	});
	
	_logger.log(log_type.information, $"Registering new user: '{credentials[$ "username"]}'");
	return user_account_client.register_async(credentials);
}
