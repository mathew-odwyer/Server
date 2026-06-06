/// @description Gets a player that matches the specified `identifier`.
/// @param {String} identifier The identifier of the player.
/// @returns {Id.Instance.obj_player} Returns the player instance that matches the specified `identifier`; or `noone` if one was not found.
function player_get_by_identifier(identifier)
{
    with (obj_player)
    {
        if (self.identifier == identifier)
        {
            return self;
        }
    }

    return noone;
}
