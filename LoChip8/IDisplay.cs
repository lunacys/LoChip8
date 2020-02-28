namespace LoChip8
{
    public interface IDisplay
    {
        void Clear();
        /// <summary>
        /// Draws sprite at specified position.
        /// Returns True if any set pixels are changed to unset (using XOR), and False otherwise
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <returns>True if any set pixels are changed to unset, and False otherwise</returns>
        bool DrawSprite(Sprite sprite, int positionX, int positionY);
    }
}