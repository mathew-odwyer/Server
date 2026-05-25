#!/bin/bash

# Xvfb leaves lock files behind that prevents the service from running if it has to restart
# This line simply removes the files so we can restart again.
rm -f /tmp/.X0-lock /tmp/.X11-unix/X0

Xvfb :0 -screen 0 800x600x24 &
exec env DISPLAY=:0 "$@"