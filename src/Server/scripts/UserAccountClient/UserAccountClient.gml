/// @description Provides a HTTP client for the UserAccount API.
function UserAccountClient() constructor
{
    /// @type {String}
    /// @description The user account login URL.
    static _login_url = $"{api_url}/api/UserAccount/Login";

    /// @description Sends a UserAccount/Login POST request to the Web API.
    /// @param {Struct} dto The login user request data transfer request.
    /// @returns {Struct.Promise} Returns a promise that is resolved on reply.
    static Login = function(dto)
    {
        return http_async(_login_url, "POST", dto);
    }
}

new UserAccountClient();