# LoChip8

LoChip8 is a simple interpreter of CHIP-8 written in C# and visualized with the power of MonoGame Framework.

## Current Status

It is now fully finished, all ROMs I tested work fine except for Space Invaders.

## ROMs

I took all the ROMs to test from [here](http://devernay.free.fr/hacks/chip8/).

## Build

As the projects use .NET Core you'll need to install [.NET Core SDK 3.1](https://docs.microsoft.com/en-us/dotnet/core/install/dependencies?tabs=netcore31&pivots=os-windows) or higher.

Clone the repo:

```bash
git clone https://github.com/lunacys/LoChip8.git
```

Then run the following command in the root of the repo to build the solution:

```
dotnet build
```

To run the GUI go to `LoChip8.DesktopGL` and run the following command:

```
dotnet run
```

## References

 - http://devernay.free.fr/hacks/chip8/C8TECH10.HTM
 - http://mattmik.com/files/chip8/mastering/chip8.html
