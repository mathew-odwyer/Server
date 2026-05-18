/// @description Represents an animation that can be played via an animation_system.
/// @param {Array<Real>} frames The array of individual frame indices for this animation.
/// @param {Real} spd The speed for this animation.
function Animation(frames, spd = 4) constructor
{
    /// @type {Array<Real>}
    /// @description The frames for this animation.
    _frames = frames;

    /// @type {Real}
    /// @description The FPS this animation should use.
    _fps = spd;

    /// @type {Real|Undeifned}
    /// @description The last time this animation was played.
    _time_stamp = undefined;

    /// @type {Real}
    /// @description Gets the total number of frame indices for this animation.
    /// @returns {Real} The total number of frame indices for this animation. 
    get_frame_count = function()
    {
        return array_length(_frames);
    }

    /// @type {Real}
    /// @description Gets the frame index from the specified index parameter.
    /// @returns {Real} The frame index that matches the specified index parameter.
    get_frame_index = function(index)
    {
        return _frames[index];
    }

    /// @type {Real}
    /// @description Gets the animation speed (frames per second).
    /// @returns {Real} The animation speed (frames per second).
    get_fps = function()
    {
        return _fps;
    }

    /// @type {Real}
    /// @description Gets the last time this animation was played.
    /// @returns {Real} The last time this animation was played.
    get_time_stamp = function()
    {
        return _time_stamp;
    }

    /// @description Resets the time stamp to indicate that the animation has started.
    reset_time_stamp = function()
    {
        _time_stamp = get_timer() / 1000000;
    }
}
