/// @description Handles an incoming request for actions to be performed.
/// @param {Array} queue The collection of actions to be performed.
/// @param {Struct.ClientConnection} connection The client connection that sent the request.
function player_action(queue, connection)
{
    var player = player_get(connection);

    if (player == noone)
    {
        return;
    }

    player.enqueue_actions(queue);
}
