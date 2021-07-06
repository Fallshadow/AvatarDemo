using System;
using ASeKi.system;
using UnityEngine;

namespace ASeKi.input
{
    public class InputSystem : MonoBehaviour, ISystem
    {
        [Header("Controller Input")]
        public string horizontalInput = "Horizontal";
        public string verticalInput = "Vertical";
        public KeyCode jumpInput = KeyCode.Space;
        public KeyCode sprintInput = KeyCode.LeftShift;
        public KeyCode humanInput = KeyCode.Alpha1;
        public KeyCode catInput = KeyCode.Alpha2;
        public KeyCode elephantInput = KeyCode.Alpha3;

        [Header("Input Param")] 
        public Vector3 InputVector3Param;
        public bool IsRunning;
        public bool IsJumping;
        
        public void InitSystem()
        {
            InputVector3Param = Vector3.zero;
            IsRunning = false;
            IsJumping = false;
        }
        
        public void FixedUpdateSystem()
        {
            InputHandle(); 
        }
        
        public void UpdateSystem()
        {
            InputHandle(); 
        }

        protected virtual void InputHandle()
        {
            moveInput();
            RunInput();
            JumpInput();
        }
        
        protected virtual void moveInput()
        {
            InputVector3Param.x = Input.GetAxis(horizontalInput);
            InputVector3Param.z = Input.GetAxis(verticalInput);
        }
        
        protected virtual void RunInput()
        {
            if (Input.GetKeyDown(sprintInput))
            {
                IsRunning = true;
            }
            else if (Input.GetKeyUp(sprintInput))
            {
                IsRunning = false;
            }
        }

        /// <summary>
        /// Input to trigger the Jump 
        /// </summary>
        protected virtual void JumpInput()
        {
            if (Input.GetKeyDown(jumpInput))
                IsJumping = true;
        }

        #region 外界接口

        public Vector3 GetInputVector3()
        {
            return InputVector3Param;
        }

        #endregion
    }
}