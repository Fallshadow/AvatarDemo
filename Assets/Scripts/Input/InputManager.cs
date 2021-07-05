using System;
using UnityEngine;

namespace ASeKi.input
{
    public class InputManager : SingletonMonoBehavior<InputManager>
    {
        [Header("Controller Input")]
        public string horizontalInput = "Horizontal";
        public string verticallInput = "Vertical";
        public KeyCode jumpInput = KeyCode.Space;
        public KeyCode sprintInput = KeyCode.LeftShift;
        public KeyCode humanInput = KeyCode.Alpha1;
        public KeyCode catInput = KeyCode.Alpha2;
        public KeyCode elephantInput = KeyCode.Alpha3;
        
        protected virtual void Update()
        {
            InputHandle(); 
        }
        
        protected virtual void InputHandle()
        {
            MoveInput();
            CameraInput();
            SprintInput();
            StrafeInput();
            JumpInput();
        }
    }
}