/// @description Represents an animation system used to handle a collection of animations within a single sprite.
function AnimationSystem() constructor
{
    /// @type {Struct}
    /// @description The tag to animation map.
    _tag_to_animation_map = {};

    /// @type {String}
    /// @description The current animation tag.
    _current_animation_tag = undefined;

    /// @description Adds an animation this animation_system that matches the specified tag.
    /// @param {String} tag The tag of the animation.
    /// @param {Struct.Animation} animation The animation to add to this animation_system.
    add_animation = function(tag, animation)
    {
        _tag_to_animation_map[$ tag] = animation;
    }

    /// @description Sets the current animation to an animation that matches the specified tag.
    /// @param {String} tag The tag that matches the animation to play.
    set_animation = function(tag)
    {
        if (_current_animation_tag == tag)
        {
            return;
        }

        var anim = _tag_to_animation_map[$ tag];

        if (is_undefined(anim))
        {
            return;
        }

        anim.reset_time_stamp();
        _current_animation_tag = tag;
    }

    /// @description Updates the current animation.
    /// @param {Id.Instance} inst The instance to process.
    step = function(inst)
    {
        var current_animation = _tag_to_animation_map[$ _current_animation_tag];

        if (is_undefined(current_animation))
        {
            return;
        }

        var system_stamp = get_timer() / 1000000;
        
        var current_stamp = current_animation.get_time_stamp();   
        var frame_count = current_animation.get_frame_count();
        var anim_fps = current_animation.get_fps();

        var frame_index = floor((system_stamp - current_stamp) * anim_fps) % frame_count;

        inst.image_index = current_animation.get_frame_index(frame_index) - 1;
    }
}
