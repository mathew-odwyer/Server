/// @description Initialize default parameters.

/// @instancevar {Struct.ClientConnection} connection The client connection associated with the player.

event_inherited();

/// @description Sends a JSON-RPC 2.0 notification to the `player`.
/// @param {String} procedure The procedure to send to the `player`.
/// @param {Any} params The parameters of the procedure to send to the `player`
notify = function(procedure, params)
{
    obj_server.notify(connection, procedure, params);
}
