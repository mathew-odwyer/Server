Promise.UnhandledRejectionCallback = function(error)
{
	if (is_struct(error))
	{
		if (is_instanceof(error, TimeoutError))
		{
			Logger.Log(log_type.error, $"Timeout Error: {error.message}");
		}
		else if (is_instanceof(error, HttpError))
		{
			Logger.Log(log_type.error, $"HTTP Error: {error.message}");
		}
		else if (is_instanceof(error, Error))
		{
			Logger.Log(log_type.warning, $"Unhandled rejection: {error.message}");
		}
	}
	else if (is_string(error))
	{
		Logger.Log(log_type.warning, $"Unhandled rejection: {error}");
	}
	else
	{
		Logger.Log(log_type.error, "Unhandled Rejection");
	}
};