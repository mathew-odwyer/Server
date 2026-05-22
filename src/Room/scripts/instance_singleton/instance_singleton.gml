/// @description Ensures that the specific `obj` is a singleton.
/// @param {Asset.GMObject} obj The object to ensure is a singleton.
function instance_singleton(obj)
{
	if (instance_number(obj) > 1)
	{
		instance_destroy(obj);
		exit;
	}
}
