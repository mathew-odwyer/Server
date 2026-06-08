/// @type {Real}
/// @description Shorthand for writing `* game_get_speed(gamespeed_fps)`.
#macro seconds * game_get_speed(gamespeed_fps)

/// @type {Real}
/// @description Shorthand for writing `* 60 * game_get_speed(gamespeed_fps)`.
#macro minutes * 60 * game_get_speed(gamespeed_fps)

/// @type {String}
/// @description Type identifier for a real (floating-point) number, including NaN and infinity values.
#macro t_real "number"

/// @type {String}
/// @description Type identifier for a string value.
#macro t_string "string"

/// @type {String}
/// @description Type identifier for an array.
#macro t_array "array"

/// @type {String}
/// @description Type identifier for a boolean value.
#macro t_bool "bool"

/// @type {String}
/// @description Type identifier for a 32-bit integer value.
#macro t_int32 "int32"

/// @type {String}
/// @description Type identifier for a 64-bit integer value.
#macro t_int64 "int64"

/// @type {String}
/// @description Type identifier for a pointer value.
#macro t_ptr "ptr"

/// @type {String}
/// @description Type identifier for an undefined value.
#macro t_undefined "undefined"

/// @type {String}
/// @description Type identifier for a null value.
#macro t_null "null"

/// @type {String}
/// @description Type identifier for a function or method reference.
#macro t_method "method"

/// @type {String}
/// @description Type identifier for a struct reference.
#macro t_struct "struct"

/// @type {String}
/// @description Type identifier for a handle reference.
#macro t_ref "ref"

/// @type {String}
/// @description Type identifier for an unknown value type.
#macro t_unknown "unknown"

/// @description Throws an exception if `parameter` is not of the specified type.
/// @param {Any} parameter The value to validate.
/// @param {String} name The parameter name used in exception messages.
/// @param {String} expected A type identifier describing the required type.
/// @returns {Bool} Returns `true` if the value matches the expected type.
function assert_type(parameter, name, expected)
{
    if (typeof(expected) == t_struct)
    {
        if (typeof(parameter) != t_struct)
        {
            throw $"Parameter '{name}' type mismatch: expected struct but was '{typeof(parameter)}'";
        }

        var keys = struct_get_names(expected);

        for (var i = 0; i < array_length(keys); i++)
        {
            var key = keys[i];
            assert_type(parameter[$ key], name + "." + key, expected[$ key]);
        }

        return true;
    }

    if (typeof(parameter) != expected)
    {
        throw $"Parameter '{name}' type mismatch: expected type '{expected}' but was '{typeof(parameter)}'";
    }

    return true;
}
