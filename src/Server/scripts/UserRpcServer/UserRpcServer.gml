function UserRpcServer() constructor
{	
	static Validate = function(token)
	{
		if (string_empty(token))
		{
			throw new UnauthorizedError("Invalid or expired client token.");
		}
		
		var dto = {
			client_token: token,
		};
		
		Logger.Log(log_type.trace, "Validating client token...");
		
		return UserAccountClient.Validate(dto)
			.next(function(result)
			{
				Logger.Log(log_type.trace, "Client token validated!");
				
				// TODO: Store the access token for the user in cache
				// TODO: Fetch all players for the user w/ their access token
				// TODO: Return all players to the client so they can decide who to play with
			})
			.fail(function(error)
			{
				Logger.Log(log_type.trace, "Failed to validate client token!");
				
				var errors = http_parse_error(error.data);
				throw new UnauthorizedError(errors.message);
			});
	}
}

new UserRpcServer();