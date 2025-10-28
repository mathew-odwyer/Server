/// @description Provides a user account API client.
/// @param {Struct} options The client options.
function UserAccountClient(options) constructor
{
    /// @type {String}
    /// @description The register user URL.
    static _register_url = $"{api_url}/api/UserAccount/Register";

    /// @type {String}
    /// @description The login user URL.
    static _login_url = $"{api_url}/api/UserAccount/Login";

    /// @type {Struct}
    /// @description The client options
    _options = options;

    /// @description Registers a new user account.
    /// @param {Struct} dto The data transfer object containing the registration details.
    /// @returns {Struct.Promise} Returns a promise that is resolved when the API responds.
    register_async = function(dto)
    {
        return http_async(_register_url, "POST", dto, _options);
    }

    /// @description Authenticates a potential user.
    /// @param {Struct} dto The data transfer object containing the users credentials.
    /// @returns {Struct.Promise} Returns a promise that is resolved when the API responds.
    login_async = function(dto)
    {
        return http_async(_login_url, "POST", dto, _options);
    }
}
