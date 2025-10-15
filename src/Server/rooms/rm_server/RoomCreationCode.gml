Promise.UnhandledRejectionCallback = function(error)
{
	if (is_struct(error))
	{
		if (is_instanceof(error, SocketError))
		{
			Logger.Log(log_type.error, "Unhandled SocketError: " + string(error.message));
		}
		else if (is_instanceof(error, TimeoutError))
		{
			Logger.Log(log_type.error, $"Timeout Error: {error.message}");
		}
		else
		{
			Logger.Log(log_type.warning, "Unhandled rejection: " + string(error.message));
		}
	}
	else
	{
		Logger.Log(log_type.warning, "Unhandled rejection: " + string(error.message));
	}
};