/// @description Initialize default parameters.

/// @instancevar {Real} min_distance The minimum distance of the audio falloff.
/// @instancevar {Real} max_distance The maximum distance of the audio falloff.
/// @instancevar {Asset.GMSound} sound The sound to be played by the entity.
/// @instancevar {Constant.Color} light_color The color of the light.
/// @instancevar {Bool} light_enabled Indicates whether this entity emits a light.
/// @instancevar {Real} light_scale How far the light extends.
/// @instancevar {Real} light_amplitude How much the light intensity varies. 
/// @instancevar {Real} light_frequency How fast the light intensity varies.
/// @instancevar {Real} light_intensity The intensity of the light.

/// @description The default audio emitter for the entity.
_emitter = audio_emitter_create();

/// @description Calculates the light intensity factor.
/// @param {Real} ambient_strength The ambient light strength.
calculate_light_intensity = function(ambient_strength)
{
	var delta = current_time * 0.001;
	var flicker = 1.0 + sin(delta * light_frequency) * light_amplitude;
	var intensity = light_intensity * flicker * ambient_strength;
	
	return intensity;
}

var min_dist = min_distance * cell_width;
var max_dist = max_distance * cell_height;

audio_falloff_set_model(audio_falloff_linear_distance);
audio_emitter_position(_emitter, x, y, 0);
audio_emitter_falloff(_emitter, min_dist, max_dist, 1);

if (sound != noone)
{
    audio_play_sound_on(_emitter, sound, true, 0);
}

// Debug view for entities
dbg_view($"Entity: {object_get_name(object_index)}: '{id}'", false);

dbg_section("Lighting", false);

dbg_checkbox(ref_create(self, nameof(light_enabled)), "Enabled");

dbg_text_separator("Properties");
dbg_color(ref_create(self, nameof(light_color)), "Color");
dbg_slider(ref_create(self, nameof(light_intensity)), 0.1, 5.0, "Intensity", 0.05);
dbg_slider(ref_create(self, nameof(light_scale)), 0.1, 10.0, "Scale", 0.05);

dbg_text_separator("Flickering");
dbg_slider(ref_create(self, nameof(light_amplitude)), 0.1, 1.0, "Amplitude", 0.1);
dbg_slider(ref_create(self, nameof(light_frequency)), 1, 16, "Frequency", 0.5);