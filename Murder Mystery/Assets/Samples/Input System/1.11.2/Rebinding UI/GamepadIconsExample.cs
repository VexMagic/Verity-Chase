using System;
using UnityEngine.UI;

////TODO: have updateBindingUIEvent receive a control path string, too (in addition to the device layout name)

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    /// <summary>
    /// This is an example for how to override the default display behavior of bindings. The component
    /// hooks into <see cref="RebindActionUI.updateBindingUIEvent"/> which is triggered when UI display
    /// of a binding should be refreshed. It then checks whether we have an icon for the current binding
    /// and if so, replaces the default text display with an icon.
    /// </summary>
    public class GamepadIconsExample : MonoBehaviour
    {
        public KeyboardIcons keyboard;
        public GamepadIcons xbox;
        public GamepadIcons ps4;

        protected void OnEnable()
        {
            // Hook into all updateBindingUIEvents on all RebindActionUI components in our hierarchy.
            var rebindUIComponents = transform.GetComponentsInChildren<RebindActionUI>();
            foreach (var component in rebindUIComponents)
            {
                component.updateBindingUIEvent.AddListener(OnUpdateBindingDisplay);
                component.UpdateBindingDisplay();
            }
        }

        protected void OnUpdateBindingDisplay(RebindActionUI component, string bindingDisplayString, string deviceLayoutName, string controlPath)
        {
            if (string.IsNullOrEmpty(deviceLayoutName) || string.IsNullOrEmpty(controlPath))
                return;

            var icon = GetIcon(deviceLayoutName, controlPath);

            var textComponent = component.bindingText;

            // Grab Image component.
            var imageGO = textComponent.transform.parent.Find("ActionBindingIcon");
            var imageComponent = imageGO.GetComponent<Image>();

            if (icon != null)
            {
                textComponent.gameObject.SetActive(false);
                imageComponent.sprite = icon;
                imageComponent.gameObject.SetActive(true);
            }
            else
            {
                textComponent.gameObject.SetActive(true);
                imageComponent.gameObject.SetActive(false);
            }
        }

        public Sprite GetIcon(string deviceLayoutName, string controlPath)
        {
            var icon = default(Sprite);
            if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "DualShockGamepad"))
                icon = ps4.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Gamepad"))
                icon = xbox.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Keyboard"))
            {
                icon = keyboard.GetSprite(controlPath);
            }
            return icon;
        }

        [Serializable]
        public struct GamepadIcons
        {
            public Sprite buttonSouth;
            public Sprite buttonNorth;
            public Sprite buttonEast;
            public Sprite buttonWest;
            public Sprite startButton;
            public Sprite selectButton;
            public Sprite leftTrigger;
            public Sprite rightTrigger;
            public Sprite leftShoulder;
            public Sprite rightShoulder;
            public Sprite dpad;
            public Sprite dpadUp;
            public Sprite dpadDown;
            public Sprite dpadLeft;
            public Sprite dpadRight;
            public Sprite leftStick;
            public Sprite rightStick;
            public Sprite leftStickPress;
            public Sprite rightStickPress;

            public Sprite GetSprite(string controlPath)
            {
                // From the input system, we get the path of the control on device. So we can just
                // map from that to the sprites we have for gamepads.
                switch (controlPath)
                {
                    case "buttonSouth": return buttonSouth;
                    case "buttonNorth": return buttonNorth;
                    case "buttonEast": return buttonEast;
                    case "buttonWest": return buttonWest;
                    case "start": return startButton;
                    case "select": return selectButton;
                    case "leftTrigger": return leftTrigger;
                    case "rightTrigger": return rightTrigger;
                    case "leftShoulder": return leftShoulder;
                    case "rightShoulder": return rightShoulder;
                    case "dpad": return dpad;
                    case "dpad/up": return dpadUp;
                    case "dpad/down": return dpadDown;
                    case "dpad/left": return dpadLeft;
                    case "dpad/right": return dpadRight;
                    case "leftStick": return leftStick;
                    case "rightStick": return rightStick;
                    case "leftStickPress": return leftStickPress;
                    case "rightStickPress": return rightStickPress;
                }
                return null;
            }
        }

        [Serializable]
        public struct KeyboardIcons
        {
            public Sprite unknown;
            public Sprite buttonQ;
            public Sprite buttonW;
            public Sprite buttonE;
            public Sprite buttonR;
            public Sprite buttonT;
            public Sprite buttonY;
            public Sprite buttonU;
            public Sprite buttonI;
            public Sprite buttonO;
            public Sprite buttonP;
            public Sprite buttonA;
            public Sprite buttonS;
            public Sprite buttonD;
            public Sprite buttonF;
            public Sprite buttonG;
            public Sprite buttonH;
            public Sprite buttonJ;
            public Sprite buttonK;
            public Sprite buttonL;
            public Sprite buttonZ;
            public Sprite buttonX;
            public Sprite buttonC;
            public Sprite buttonV;
            public Sprite buttonB;
            public Sprite buttonN;
            public Sprite buttonM;
            public Sprite buttonUp;
            public Sprite buttonDown;
            public Sprite buttonLeft;
            public Sprite buttonRight;
            public Sprite button1;
            public Sprite button2;
            public Sprite button3;
            public Sprite button4;
            public Sprite button5;
            public Sprite button6;
            public Sprite button7;
            public Sprite button8;
            public Sprite button9;
            public Sprite button0;
            public Sprite buttonMinus;
            public Sprite buttonPlus;
            public Sprite buttonPeriod;
            public Sprite buttonComma;
            public Sprite buttonLessThan;
            public Sprite buttonTab;
            public Sprite buttonCapsLock;
            public Sprite buttonEnter;
            public Sprite buttonShift;
            public Sprite buttonControl;
            public Sprite buttonAlt;
            public Sprite buttonSpace;
            public Sprite buttonEscape;

            public Sprite GetSprite(string controlPath)
            {
                // From the input system, we get the path of the control on device. So we can just
                // map from that to the sprites we have for gamepads.
                switch (controlPath)
                {
                    default: return unknown;
                    case "q": return buttonQ;
                    case "w": return buttonW;
                    case "e": return buttonE;
                    case "r": return buttonR;
                    case "t": return buttonT;
                    case "y": return buttonY;
                    case "u": return buttonU;
                    case "i": return buttonI;
                    case "o": return buttonO;
                    case "p": return buttonP;
                    case "a": return buttonA;
                    case "s": return buttonS;
                    case "d": return buttonD;
                    case "f": return buttonF;
                    case "g": return buttonG;
                    case "h": return buttonH;
                    case "j": return buttonJ;
                    case "k": return buttonK;
                    case "l": return buttonL;
                    case "z": return buttonZ;
                    case "x": return buttonX;
                    case "c": return buttonC;
                    case "v": return buttonV;
                    case "b": return buttonB;
                    case "n": return buttonN;
                    case "m": return buttonM;
                    case "upArrow": return buttonUp;
                    case "downArrow": return buttonDown;
                    case "leftArrow": return buttonLeft;
                    case "rightArrow": return buttonRight;
                    case "1": return button1;
                    case "2": return button2;
                    case "3": return button3;
                    case "4": return button4;
                    case "5": return button5;
                    case "6": return button6;
                    case "7": return button7;
                    case "8": return button8;
                    case "9": return button9;
                    case "0": return button0;
                    case "minus": return buttonMinus;
                    case "plus": return buttonPlus;
                    case "period": return buttonPeriod;
                    case "comma": return buttonComma;
                    case "tab": return buttonTab;
                    case "capsLock": return buttonCapsLock;
                    case "enter": return buttonEnter;
                    case "leftShift":
                    case "rightShift": return buttonShift;
                    case "leftCtrl":
                    case "rightCtrl": return buttonControl;
                    case "leftAlt":
                    case "rightAtrl": return buttonAlt;
                    case "space": return buttonSpace;
                    case "escape": return buttonEscape;
                }
            }
        }
    }
}
