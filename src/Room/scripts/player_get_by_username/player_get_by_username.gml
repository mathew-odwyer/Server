/// @description Gets a player that matches the specified `username`.
/// @param {String} username The name of the player to fetch.
/// @returns {Id.Instance.obj_player} Returns the player that matches the specified `username`; or `noone` if no player was found.
function player_get_by_username(username)
{
    with (obj_player)
    {
        if (string_upper(self.name) == string_upper(username))
        {
            return self;
        }
    }

    return noone;
}