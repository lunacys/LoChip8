﻿using System;

namespace LoChip8
{
    public class Keypad
    {
        public event EventHandler<byte> KeyDown;
        public event EventHandler<byte> KeyUp;
        
        private bool[] _pressedKeys;

        public bool[] PressedKeys => _pressedKeys;

        public Keypad()
        {
            _pressedKeys = new bool[16];
        }
        
        public void SendKey(byte key, bool isDown)
        {
            if (key > 0x0F)
                throw new ArgumentException("Only 16 keys mode is supported", nameof(key));

            _pressedKeys[key] = isDown;
            if (isDown)
                KeyDown?.Invoke(this, key);
            else
                KeyUp?.Invoke(this, key);
        }

        public bool IsKeyDown(byte key)
        {
            return _pressedKeys[key];
        }
    }
}