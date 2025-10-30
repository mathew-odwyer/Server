/// @description Generates a (non-secure) unique identifier.
/// @returns {Real} Returns a (non-secure) unique identifier.
function generate_uuid()
{
	static uuid = 0;
	return uuid++;
}
