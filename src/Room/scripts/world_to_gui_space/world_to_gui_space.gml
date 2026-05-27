/// @description Converts world space coordinates to GUI space coordinates.
/// @param {Id.Camera} camera The camera to use for the conversion.
/// @param {Real} x The world space x coordinate to convert.
/// @param {Real} y The world space y coordinate to convert.
/// @returns {Any} A struct containing the converted coordinates and scale ratios:
///                   x: The GUI space x coordinate.
///                   y: The GUI space y coordinate.
///                   ratio_x: The horizontal scale ratio between GUI and camera view.
///                   ratio_y: The vertical scale ratio between GUI and camera view.
function world_to_gui_space(camera, x, y)
{
    var camera_x = camera_get_view_x(camera);
    var camera_y = camera_get_view_y(camera);

    var camera_w = camera_get_view_width(camera);
    var camera_h = camera_get_view_height(camera);

    var display_w = display_get_gui_width();
    var display_h = display_get_gui_height();

    // Get the ratio between the GUI size and the Camera view size.
    var ratio_x = display_w / camera_w;
    var ratio_y = display_h / camera_h;

    var gui_x = (x - camera_x) * ratio_x;
    var gui_y = (y - camera_y) * ratio_y;

    return {
        x: gui_x,
        y: gui_y,
        ratio_x: ratio_x,
        ratio_y: ratio_y,
    };
}