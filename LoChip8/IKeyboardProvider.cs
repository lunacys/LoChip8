namespace LoChip8
{
    public interface IKeyboardProvider
    {
        void KeyPress(byte key);
        byte KeyPress(); // TODO: Improve this method
    }
}