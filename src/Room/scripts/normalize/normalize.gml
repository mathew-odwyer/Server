/// @description Normalizes a value to a 0-1 range based on a given minimum and maximum.
/// @param {Real} value The value to normalize.
/// @param {Real} min The minimum value of the range.
/// @param {Real} max The maximum value of the range.
/// @returns {Real} The normalized value between 0 and 1.
function normalize(value, min, max)
{
    return (value - min) / (max - min);
}