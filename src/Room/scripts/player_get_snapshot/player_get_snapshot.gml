/// @description Gets a player snapshot.
/// @param {Id.Instance.obj_player|Struct} player The player instance.
/// @returns {Struct} Returns a snapshot of the specified `player`.
function player_get_snapshot(player)
{
	return {
		id: string_empty(player.identifier) ? "" : player.identifier,
		name: string_empty(player.name) ? "Player" : player.name,
		x: player.x,
		y: player.y,
	};
}