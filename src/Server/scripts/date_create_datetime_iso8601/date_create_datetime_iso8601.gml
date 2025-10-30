/// @description Creates a date time from a string (uses ISO 8601).
/// @param {String} date_time The date time represented as a string.
/// @returns {Real} Returns `date_time` converted to date time struct.
function date_create_datetime_iso8601(date_time)
{
	var year = real(string_copy(date_time, 1, 4));
	var month = real(string_copy(date_time, 6, 2));
	var day = real(string_copy(date_time, 9, 2));
	var hour = real(string_copy(date_time, 12, 2));
	var minute = real(string_copy(date_time, 15, 2));
	var second = real(string_copy(date_time, 18, 2));
	
	return date_create_datetime(year, month, day, hour, minute, second);
}
