# LoChip8

LoChip8 is a simple interpreter of CHIP-8 written in C# and visualized with the power of MonoGame Framework.

## Current Status

It is almost finished, it works well with any ROM I tested,
but the timings are messed up (registers DT and ST) and some display artifacts occured in games with XOR collision detection (inctruction DXYN).

Also the project structure is probably a mess - almost everything sits in the VirtualMachine class. Probably need to refactor it.

Most likely I will not continue working on LoChip8, instead, I started a new project - LoNES, which is an emulator for NES.

## References

http://devernay.free.fr/hacks/chip8/C8TECH10.HTM
http://mattmik.com/files/chip8/mastering/chip8.html