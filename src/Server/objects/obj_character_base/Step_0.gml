/// @description Handle movement and update state.

var xinput = _move_x;
var yinput = _move_y;

var collidables = [obj_entity_base];

if (layer_exists("Collisions"))
{
	var collisions_layer = layer_get_id("Collisions");
    var tilemap_id = layer_tilemap_get_id(collisions_layer);

    array_push(collidables, tilemap_id);
}
	
move_and_collide(xinput, yinput, collidables);

_state_machine.step();
_animation_system.step(id);
