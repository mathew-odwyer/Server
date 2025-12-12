/// @description Send actions performed to clients.

alarm[0] = tick_rate;

var length = array_length(_actions_performed);

// If no actions have been performed since the last time, no need to send updates.
if (length == 0)
{
    exit;
}

_process_tick();
array_resize(_actions_performed, 0);
