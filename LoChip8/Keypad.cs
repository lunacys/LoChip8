namespace LoChip8;

public class Keypad
{
    public event EventHandler<byte>? KeyPressed;
    public event EventHandler<byte>? KeyReleased; 
    
    public readonly bool[] PressedKeys = new bool[4 * 4];

    public void Reset()
    {
        for (int i = 0; i < PressedKeys.Length; i++)
        {
            PressedKeys[i] = false;
        }
    }

    public void SetKey(byte key, bool isDown)
    {
        if (key >= 16) return;

        PressedKeys[key] = isDown;
        
        if(isDown)
            KeyPressed?.Invoke(this, key);
        else
            KeyReleased?.Invoke(this, key);
    }
    
    public bool IsKeyDown(byte key)
        => PressedKeys[key];
}