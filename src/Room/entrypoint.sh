#!/bin/bash
 
# X Display can't open once the server has crashed or game_end has been called
# unless we delete the lock file, see: https://github.com/mathew-odwyer/Server/issues/129
rm -f /tmp/.X0-lock
 
Xvfb :0 -screen 0 800x600x24 &
 
# gm-cli's progress UI measures the terminal to redraw its spinner.
# `script` above gives it a real TTY, but a freshly-spawned pty with no
# controlling terminal to inherit from starts at a 0x0 window size. That's
# worse than no TTY at all -- "length / 0" comes out as Infinity (not NaN),
# and feeding Infinity into the line-eraser is exactly what blows the
# string up with "RangeError: Invalid string length". COLUMNS/LINES env
# vars don't help here -- Node reads real TTY size via an ioctl syscall,
# not env vars. So set the pty's size explicitly, as the first thing the
# spawned shell does, before it execs gm-cli.
exec env DISPLAY=:0 script -qec "stty rows 24 cols 80 && $*" /dev/null