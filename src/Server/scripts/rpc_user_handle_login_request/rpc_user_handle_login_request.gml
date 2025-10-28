/// @description Handles an incoming login request from the client.
/// @param {Struct} credentials The credentials used to authenticate the user.
/// @param {Struct.ClientConnection} connection The client that sent the request.
/// @returns {Struct.Promise} Returns a promise that is resolved when the user logs in.
function rpc_user_handle_login_request(credentials, connection)
{
    /// @type {Struct.Logger}
    /// @description The logger.
    static _logger = new Logger(nameof(rpc_user_handle_login_request));

    var user_account_client = new UserAccountClient({
        jsonrpc_error: true,
    });

    _logger.log(log_type.information, $"'{credentials[$ "username"]}' logging in...'");

    var dto = {
        username: credentials[$ "username"],
        password: credentials[$ "password"],
    };

    return user_account_client.login_async(dto)
        .next(method({_logger, dto, connection}, function(result)
        {
            connection[$ "access_token"] = result[$ "access_token"];
            connection[$ "refresh_token"] = result[$ "refresh_token"];

            // Convert expiration date string to ISO 8601 date.
            var expiration_date = result[$ "expiration_date"];
            var expiration_iso = date_create_datetime_iso_8601(expiration_date);

            // Calculate how many seconds until the access token expires.
            var now = date_current_datetime();
            var wait = max(0, date_second_span(now, expiration_iso));

            // After x seconds, disconnect the client connection.
            // Store it in connection for a user refresh request to cancel and reset.

            // TODO: Cancel this if the client disconnects and it's still active? Probably!
            connection[$ "access_timer"] = call_later(wait, time_source_units_seconds, connection.cleanup);

            _logger.log(log_type.trace, $"'{dto.username}' session expires in {wait}s.");

            var player_client = new PlayerClient({
                bearer: connection[$ "access_token"],
                jsonrpc_error: true,
            })

            return player_client.get_async();
        }))
        .next(method({_logger, connection}, function(result)
        {
            var players = [];
            var player = result.player;
            var snapshot = player_snapshot(player);

            with (obj_player)
            {
                array_push(players, player_snapshot(self));
                obj_server.notify(self.connection, "player.create_remote", snapshot);
            }

            /// @type {Id.Instance.obj_player}
            var inst = instance_create_layer(player.x, player.y, "Instances", obj_player);

            inst.name = player.name;
            inst.connection = connection;

            _logger.log(log_type.information, $"'{player.name}' logged in!");

            return {
                player: player,
                players: players,
                refresh_token: connection[$ "refresh_token"],
            };
        }));
}

// TODO: Move this into GMUtilities
function date_create_datetime_iso_8601(date_time)
{
    // Extract parts from ISO 8601 date-time.
    var year = real(string_copy(date_time, 1, 4));
    var month = real(string_copy(date_time, 6, 2));
    var day = real(string_copy(date_time, 9, 2));
    var hour = real(string_copy(date_time, 12, 2));
    var minute = real(string_copy(date_time, 15, 2));
    var second = real(string_copy(date_time, 18, 2));

    return date_create_datetime(year, month, day, hour, minute, second);
}
