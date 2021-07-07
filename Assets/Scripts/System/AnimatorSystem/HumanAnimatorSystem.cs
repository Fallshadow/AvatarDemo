using UnityEngine;

namespace ASeKi.system
{
    public class HumanAnimatorSystem : AnimatorSystem
    {
        private HumanAnimatorParametersStruct animPData;
        private float animationInputPSmooth = 0.2f;

        public virtual void SetParam(HumanAnimatorParametersStruct humanAnimatorParametersStructP)
        {
            animPData = humanAnimatorParametersStructP;
            animPData.InputMagnitude = SetAnimatorMoveSpeed(animPData.MoveDirection, animPData.RunningSpeed, animPData.WalkSpeed);
        }

        public override void UpdateSystem()
        {
            base.UpdateSystem();
            UpdateAnimator();
        }

        public virtual void UpdateAnimator()
        {
            if (animator == null || !animator.enabled) return;

            animator.SetBool(HumanAnimatorParameters.IsRunning, animPData.IsRunning);
            animator.SetBool(HumanAnimatorParameters.IsGrounded, animPData.IsGrounded);
            animator.SetFloat(HumanAnimatorParameters.GroundDistance, animPData.GroundDistance);
            animator.SetFloat(HumanAnimatorParameters.InputMagnitude, 
                animPData.StopMove ? 0 : animPData.InputMagnitude, animationInputPSmooth, Time.deltaTime);
        }

        public float SetAnimatorMoveSpeed(Vector3 moveDirection, float runningSpeed, float walkSpeed)
        {
            Vector3 relativeInput = transform.InverseTransformDirection(moveDirection);
            var newInput = new Vector2(relativeInput.z, relativeInput.x);
            return Mathf.Clamp(newInput.magnitude, 0, animPData.IsRunning ? runningSpeed : walkSpeed);
        }
    }

    public static class HumanAnimatorParameters
    {
        public static int InputMagnitude = Animator.StringToHash("InputMagnitude");
        public static int IsGrounded = Animator.StringToHash("IsGrounded");
        public static int IsRunning = Animator.StringToHash("IsRunning");
        public static int GroundDistance = Animator.StringToHash("GroundDistance");
    }

    public struct HumanAnimatorParametersStruct
    {
        public bool IsRunning;
        public bool IsGrounded;
        public bool StopMove;
        public float GroundDistance;
        public float InputMagnitude;
        public float RunningSpeed;
        public float WalkSpeed;
        public Vector3 MoveDirection;

        public HumanAnimatorParametersStruct(bool IsRunning, bool IsGrounded, bool StopMove,
            float GroundDistance, Vector3 MoveDirection, float RunningSpeed, float WalkSpeed)
        {
            this.GroundDistance = GroundDistance;
            this.MoveDirection = MoveDirection;
            this.IsGrounded = IsGrounded;
            this.IsRunning = IsRunning;
            this.StopMove = StopMove;
            this.RunningSpeed = RunningSpeed;
            this.WalkSpeed = WalkSpeed;
            this.InputMagnitude = 0;
        }
    }
}