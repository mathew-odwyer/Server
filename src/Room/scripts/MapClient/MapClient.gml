/// @description Provides a Map API client.
/// @param {Struct} options The client options.
function MapClient(options) constructor
{
    /// @type {String}
    /// @description The get URL.
    static _get_url = $"{api_url}/api/Map/Get";

    /// @type {Struct}
    /// @description The client options.
    _options = options;

    /// @description Gets a map that matches the specified `name`.
    /// @param {String} name The name of the map to be fetched.
    /// @returns {Struct.__Promise} Returns a promise that is resolved when the API responds.
    get_async = function(name)
    {
        return http_async($"{_get_url}?Name={name}", "GET", {}, _options);
    }
}
