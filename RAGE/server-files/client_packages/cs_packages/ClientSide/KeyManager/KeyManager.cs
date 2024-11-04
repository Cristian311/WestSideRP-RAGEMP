
using System;
using System.Threading.Tasks;
using RAGE;


namespace CHRP.Assets
{
    public class KeyManager : RAGE.Events.Script
    {
        private const int ResetTime = 250;    // Time to reset the key
        private static bool _keyStatus = true; // The state of the key

        public const int KeyMouse = 0xC0;    //    Key [~]
        public const int KeyLctrl = 0xA2;    //    Key [Left CTRL]



        public static void KeyBind(int key, Action action)
        {
            if (!Input.IsDown(key) || !_keyStatus) return;
            if (!_keyStatus) return;
            action.Invoke();
            _keyStatus = false;
            System.Threading.Tasks.Task.Delay(ResetTime).ContinueWith((task) => { _keyStatus = true; });
        }

    }
    
}