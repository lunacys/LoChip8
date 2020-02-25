using System.Diagnostics.CodeAnalysis;

namespace LoChip8
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public enum Instructions : short
	{
		/// <summary>
		/// Execute machine language subroutine at address NNN
		/// </summary>
		I_0NNN = 0,
		/// <summary>
		/// Clear the screen
		/// </summary>
		I_00E0,
		/// <summary>
		/// Return from a subroutine
		/// </summary>
		I_00EE,
		/// <summary>
		/// Jump to address NNN
		/// </summary>
		I_1NNN,
		/// <summary>
		/// Execute subroutine starting at address NNN
		/// </summary>
		I_2NNN,
		/// <summary>
		/// Skip the following instruction if the value of register VX equals NN
		/// </summary>
		I_3XNN,
		/// <summary>
		/// Skip the following instruction if the value of register VX is not equal to NN
		/// </summary>
		I_4XNN,
		/// <summary>
		/// Skip the following instruction if the value of register VX is equal to the value of register VY
		/// </summary>
		I_5XY0,
		/// <summary>
		/// Store number NN in register VX
		/// </summary>
		I_6XNN,
		/// <summary>
		/// Add the value NN to register VX
		/// </summary>
		I_7XNN,
		/// <summary>
		/// Store the value of register VY in register VX
		/// </summary>
		I_8XY0,
		/// <summary>
		/// Set VX to VX OR VY
		/// </summary>
		I_8XY1,
		/// <summary>
		/// Set VX to VX AND VY
		/// </summary>
		I_8XY2,
		/// <summary>
		/// Set VX to VX XOR VY
		/// </summary>
		I_8XY3,
		/// <summary>
		/// Add the value of register VY to register VX.
		/// Set VF to 01 if a carry occurs.
		/// Set VF to 00 if a carry does not occur.
		/// </summary>
		I_8XY4,
		/// <summary>
		/// Subtract the value of register VY from register VX.
		/// Set VF to 00 if a borrow occurs.
		/// Set VF to 01 if a borrow does not occur.
		/// </summary>
		I_8XY5,
		/// <summary>
		/// Store the value of register VY shifted right one bit in register VX.
		/// Set register VF to the least significant bit prior to the shift.
		/// </summary>
		I_8XY6,
		/// <summary>
		/// Set register VX to the value of VY minus VX.
		/// Set VF to 00 if a borrow occurs.
		/// Set VF to 01 if a borrow does not occur.
		/// </summary>
		I_8XY7,
		/// <summary>
		/// Store the value of register VY shifted left one bit in register VX.
		/// Set register VF to the most significant bit prior to the shift.
		/// </summary>
		I_8XYE,
		/// <summary>
		/// Skip the following instruction if the value of register VX is not equal to the value of register VY
		/// </summary>
		I_9XY0,
		/// <summary>
		/// Store memory address NNN in register I
		/// </summary>
		I_ANNN,
		/// <summary>
		/// Jump to address NNN + V0
		/// </summary>
		I_BNNN,
		/// <summary>
		/// Set VX to a random number with a mask of NN
		/// </summary>
		I_CXNN,
		/// <summary>
		/// Draw a sprite at position VX, VY with N bytes of sprite data starting at the address stored in I.
		/// Set VF to 01 if any set pixels are changed to unset, and 00 otherwise.
		/// </summary>
		I_DXYN,
		/// <summary>
		/// Skip the following instruction if the key corresponding to the hex value currently stored in register VX is pressed
		/// </summary>
		I_EX9E,
		/// <summary>
		/// Skip the following instruction if the key corresponding to the hex value currently stored in register VX is not pressed
		/// </summary>
		I_EXA1,
		/// <summary>
		/// Store the current value of the delay timer in register VX
		/// </summary>
		I_FX07,
		/// <summary>
		/// Wait for a keypress and store the result in register VX
		/// </summary>
		I_FX0A,
		/// <summary>
		/// Set the delay timer to the value of register VX
		/// </summary>
		I_FX15,
		/// <summary>
		/// Set the sound timer to the value of register VX
		/// </summary>
		I_FX18,
		/// <summary>
		/// Add the value stored in register VX to register I
		/// </summary>
		I_FX1E,
		/// <summary>
		/// Set I to the memory address of the sprite data corresponding to the hexadecimal digit stored in register VX
		/// </summary>
		I_FX29,
		/// <summary>
		/// Store the binary-coded decimal equivalent of the value stored in register VX at addresses I, I+1, and I+2
		/// </summary>
		I_FX33,
		/// <summary>
		/// Store the values of registers V0 to VX inclusive in memory starting at address I.
		/// I is set to I + X + 1 after operation.
		/// </summary>
		I_FX55,
		/// <summary>
		/// Fill registers V0 to VX inclusive with the values stored in memory starting at address I.
		/// I is set to I + X + 1 after operation.
		/// </summary>
		I_FX65
	}
}