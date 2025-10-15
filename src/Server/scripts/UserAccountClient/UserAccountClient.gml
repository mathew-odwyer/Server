function UserAccountClient(base_url = api_url) constructor
{
	static _validate_url = $"{base_url}/api/UserAccount/Validate";
	
	static Validate = function(dto)
	{
		return http_async(_validate_url, "POST", dto);
	}
}

new UserAccountClient();