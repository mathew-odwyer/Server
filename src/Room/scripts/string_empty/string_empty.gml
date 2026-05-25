/// @description Determines whether the specified `str` parameter is `undefined`, empty or consists of only whitespace characters.
/// @param {String|Undefined} str The string to test (or `undefined` to ultimately return `true`).
/// @returns {Bool} Returns `true` if the specified `str` is `undefined`, `empty` or consits of only whitespace characters; otherwise, `false`.
function string_empty(str)
{	
	if (is_undefined(str))
	{
		return true;
	}
	
	return string_trim(str) == "";
}
