/// @description Gets a player snapshot.
/// @param {Id.Instance.obj_player|Struct} player The player instance.
/// @returns {Struct} Returns a snapshot of the specified `player`.
function player_get_snapshot(player)
{
	return {
		name: player.name ?? "Player",
		x: player.x,
		y: player.y,
	};
}