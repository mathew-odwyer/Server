/// @description Provides client-side methods for user account management API interactions.
/// @param {String} base_url The base URL of the API.
function UserAccountClient(base_url = api_url) constructor
{
    /// @type {String}
    /// @description The register user URL.
    static _register_url = $"{base_url}/UserAccount/Register";

    /// @description Sends a user registration request to the API.
    /// @param {Struct} dto The data transfer object containing user registration details.
    /// @returns {Struct.Promise} Returns a promise that resolves with the API response.
    static RegisterAsync = function(dto)
    {
        return http_async(_register_url, "POST", dto);
    }
}

new UserAccountClient();