#!/bin/bash

# X Display can't open once the server has crashed or game_end has been called
# unless we delete the lock file, see: https://github.com/mathew-odwyer/Server/issues/129
rm -f /tmp/.X0-lock

Xvfb :0 -screen 0 800x600x24 &
exec env DISPLAY=:0 "$@"
