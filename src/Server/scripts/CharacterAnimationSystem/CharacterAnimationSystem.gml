/// @description Represents a character animation system used to handle character animations within a single sprite.
function CharacterAnimationSystem() : AnimationSystem() constructor
{
    add_animation("IdleDown", new Animation([2]));
    add_animation("IdleLeft", new Animation([25]));
    add_animation("IdleRight", new Animation([48]));
    add_animation("IdleUp", new Animation([71]));

    add_animation("WalkDown", new Animation([1, 2, 3, 2]));
    add_animation("WalkLeft", new Animation([24, 25, 26, 25]));
    add_animation("WalkRight", new Animation([47, 48, 49, 48]));
    add_animation("WalkUp", new Animation([70, 71, 72, 71]));

    set_animation("IdleDown");
}
