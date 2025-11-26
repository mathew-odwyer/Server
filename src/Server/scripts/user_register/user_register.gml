/// @descriptions Registers a new user account.
/// @param {Struct} credentials The user account credentials.
/// @returns {Struct.__Promise} Returns a promise that is resolved when the API responds.
function user_register(credentials)
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(user_register));
	
	var user_account_client = new UserAccountClient({
		jsonrpc_error: true,
	});
	
	_logger.log(log_type.information, $"Registering new user: '{credentials[$ "username"]}'");
	return user_account_client.register_async(credentials);
}
