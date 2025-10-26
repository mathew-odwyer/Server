/// @description Enumerates different types of character parts.
enum character_part_type
{
	hair = 0,
	head = 1,
	top = 2,
	bottom = 3,
	size = 4,
}

/// @description Represents a player customization part with variations and colors.
/// @param {Enum.character_part_type} type The type of the customization part.
/// @param {Real} max_variations The maximum number of variations available for this part.
/// @param {Real} max_colors The maximum number of colors available for this part.
function CharacterCustomizationPart(type, max_variations, max_colors) constructor
{
	/// @type {Enum.character_part_type}
	/// @description The type of the customization part.
	_type = min(type, character_part_type.size);

	/// @type {Real}
	/// @description The maximum number of variations available for this part.
	_max_variations = max_variations;

	/// @type {Real}
	/// @description The maximum number of colors available for this part.
	_max_colors = max_colors;

	/// @type {Real}
	/// @description The index of the current variation for this customization part.
	_variation_index = 0;
	
	/// @type {Real}
	/// @description The index of the current color for this customization part.
	_color_index = 0;
	
	/// @type {Bool}
	/// @description  Indicates whether the variation or color has changed.
	_is_dirty = false;
	
	/// @type {Asset.GMSprite|Undefined}
	/// @description The current sprite asset to be used for this customization part.
	_sprite_asset = undefined;

	/// @description Sets the variation index for this customization part.
	/// @param {Real} index The variation index.
	set_variation = function(index)
	{
		_variation_index = min(index, _max_variations - 1);
		dirty = true;
	}
	
	/// @description Sets the color index for this customization part.
	/// @param {Real} index The color index.
	set_color = function(index)
	{
		_color_index = min(index, _max_colors - 1);
		dirty = true;
	}

	/// @description Gets the variation index for this customization part.
    /// @returns {Real} The variation index for this customization part.
    get_variation = function()
    {
        return _variation_index;
    }

	/// @description Gets the color index for this customization part.
    /// @returns {Real} The color index for this customization part.
    get_color = function()
    {
        return _color_index;
    }

	/// @description Gets the sprite index for this customization part based on the current variation and color.
	/// @returns {Asset.GMSprite|Undefined} Returns the sprite index for this customization part based on the current variation and color.
	get_sprite = function()
	{
		static type_to_name_map = [
			[character_part_type.hair, "HAIR"],
			[character_part_type.head, "HEAD"],
			[character_part_type.top, "TOP"],
			[character_part_type.bottom, "BOTTOM"],
		];
		
		if (_is_dirty || _sprite_asset == undefined)
		{
			var type = string_lower(type_to_name_map[_type][1]);
			
			_sprite_asset = asset_get_index($"spr_character_{type}_{_variation_index}_{_color_index}");
			_is_dirty = false;
		}

		/// @feather ignore once GM1045
		return _sprite_asset;
	}
}
