// TODO: Unit tests
function user_logged_in_event_handler(event)
{
    /// @type {Struct.Logger}
    /// @description The logger.
    static _logger = new Logger(nameof(user_logged_in_event_handler));

    /// @type {Struct}
    /// @description The schema of the event.
    static _schema = {
        UserAccountId: t_string,
        AccessToken: t_string,
    };

    assert_type(event, nameof(event), _schema);

    var identifier = event.UserAccountId;
    var access_token = event.AccessToken;

    if (player_exists(identifier))
    {
        _logger.log(log_type.warning, $"Player with ID '{identifier}' already exists in this room.");
        return;
    }

    _logger.log(log_type.debug, $"Adding player to room with ID: '{identifier}'...");

    var player_client = new PlayerClient({
        bearer: access_token,
    });

    player_client
        .get_async()
        .next(method({_logger}, function(response)
        {
            var players = [];

            /// @type {Id.Instance.obj_player}
            var inst = instance_create_layer(response.x, response.y, "Instances", obj_player);

            try
            {
                inst.name = response.name;
                inst.identifier = response.id;

                var snapshot = player_snapshot(inst);

                with (obj_player)
                {
                    if (self.identifier == inst.identifier)
                    {
                        continue;
                    }

                    notify("room.player.join", snapshot);
                    array_push(players, player_snapshot(self));
                }

                inst.notify("room.player.synchronize", {
                    player: snapshot,
                    players: players,
                    map: {
                        data: obj_map_loader.map_data,
                    }
                });
            }
            catch (ex)
            {
                instance_destroy(inst);
                throw ex;
            }

            _logger.log(log_type.information, $"Player joined room with ID: '{inst.identifier}'");
        }))
        .fail(method({_logger, identifier}, function(error)
        {
            _logger.log(log_type.error, $"Failed to add player '{identifier}' to room: '{error}'");
        }));
}