/// @description Represents a player customizer that manages various character customization parts.
function CharacterCustomizer() constructor
{
	/// @type {Struct.CharacterCustomizationPart}
	/// @description The hair customization part.
	hair = new CharacterCustomizationPart(character_part_type.hair, 12, 8);

	/// @type {Struct.CharacterCustomizationPart}
	/// @description The head customization part.
	head = new CharacterCustomizationPart(character_part_type.head, 5, 3);

	/// @type {Struct.CharacterCustomizationPart}
	/// @description The top customization part.
	top = new CharacterCustomizationPart(character_part_type.top, 12, 5);

	/// @type {Struct.CharacterCustomizationPart}
	/// @description The bottom customization part.
	bottom = new CharacterCustomizationPart(character_part_type.bottom, 8, 5);
	
	/// @description Draws the player character with the current customization settings.
    /// @param {Real} index The index of the sprite.
    /// @param {Real} x The x-coordinate of the position to draw.
    /// @param {Real} y The y-coordinate of the position to draw.
    draw = function(index, x, y)
    {
		var head_sprite = head.get_sprite();
		var hair_sprite = hair.get_sprite();
		var top_sprite = top.get_sprite();
		var bottom_sprite = bottom.get_sprite();
		var shadow = spr_character_shadow_0_0;
		
        draw_sprite(shadow, index, x, y);
		
		if (head_sprite != -1)
		{
			draw_sprite(head_sprite, index, x, y);
		}
		
		if (hair_sprite != -1)
		{
			draw_sprite(hair_sprite, index, x, y);
		}
		
		if (top_sprite != -1)
		{
			draw_sprite(top_sprite, index, x, y);
		}
			
		if (bottom_sprite != -1)
		{
			draw_sprite(bottom_sprite, index, x, y);
		}
    }
}
