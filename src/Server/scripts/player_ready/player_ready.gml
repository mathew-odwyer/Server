function player_ready(_, connection)
{
    /// @type {Struct.Logger}
    /// @description The logger.
    static _logger = new Logger(nameof(player_ready));

    var player = player_get_by_connection(connection);

    _logger.log(log_type.debug, $"Readying player: '{player.name}'");

    if (player == noone || player.ready)
    {
        return;
    }

    player.ready = true;

    _logger.log(log_type.debug, $"Player ready: '{player.name}'");
    EventAggregator.Publish("PLAYER_LOGGED_IN", player);
}
