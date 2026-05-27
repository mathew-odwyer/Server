/// @description Determines whether the specified `inst` is within the view boundaries of the specified `camera`.
/// @param {Id.Instance} inst The instance to check against.
/// @param {Id.Camera} camera The camera to use when performing the check.
/// @returns {Bool} Returns `true` if the specified `inst` is within the view boundaries; otherwise, `false`.
function instance_in_view(inst, camera)
{
    var camera_x = camera_get_view_x(camera);
    var camera_y = camera_get_view_y(camera);
    var camera_w = camera_get_view_width(camera);
    var camera_h = camera_get_view_height(camera);

    var inst_x = inst.x;
    var inst_y = inst.y;

    var rect_x1 = camera_x;
    var rect_y1 = camera_y;
    var rect_x2 = camera_x + camera_w;
    var rect_y2 = camera_y + camera_h;

    return point_in_rectangle(inst_x, inst_y, rect_x1, rect_y1, rect_x2, rect_y2);
}
