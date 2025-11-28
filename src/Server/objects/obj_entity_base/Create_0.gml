/// @description Initialize default parameters.

/// @instancevar {Real} min_distance The minimum distance of the audio falloff.
/// @instancevar {Real} max_distance The maximum distance of the audio falloff.
/// @instancevar {Asset.GMSound} sound The sound to be played by the entity.

/// @type {Id.AudioEmitter}
/// @description The default audio emitter for the entity.
_emitter = audio_emitter_create();

var min_dist = min_distance * cell_width;
var max_dist = max_distance * cell_height;

audio_falloff_set_model(audio_falloff_linear_distance);
audio_emitter_position(_emitter, x, y, 0);
audio_emitter_falloff(_emitter, min_dist, max_dist, 1);

if (sound != noone)
{
    audio_play_sound_on(_emitter, sound, true, 0);
}
