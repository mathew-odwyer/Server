function player_exists(username)
{
    return player_get_by_username(username) != noone;
}