/// @description Provides player API client.
/// @param {Struct} options The client options.
function PlayerClient(options) constructor
{
    /// @type {Struct.Logger}
    /// @description The logger.
    static _logger = new Logger(nameof(PlayerClient));

    /// @type {String}
    /// @description The get URL.
    static _get_url = $"{api_url}/api/Player/Get";

    /// @type {String}
    /// @description The update URL.
    static _update_url = $"{api_url}/api/Player/Update";

    /// @type {Struct}
    /// @description The client options.
    _options = options;

    /// @description Gets the player.
    /// @returns {Struct.Promise} Returns a promise that is resolved when the API responds.
    get_async = function()
    {
        _logger.log(log_type.debug, "Fetching player...");
        return http_async(_get_url, "GET", {}, _options);
    }

    /// @description Updates the player.
    /// @param {Struct} dto The data transfer object containing the player data to be updated.
    /// @returns {Struct.Promise} Returns a promise that is resolved when the API responds.
    update_async = function(dto)
    {
        _logger.log(log_type.debug, "Updating player...");
        return http_async(_update_url, "PATCH", dto, _options);
    }
}
