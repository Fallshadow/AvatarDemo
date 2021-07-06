using UnityEngine;

namespace ASeKi.system
{
    public class HumanAnimatorSystem : AnimatorSystem
    {
        public virtual void UpdateAnimator()
        {
            if (animator == null || !animator.enabled) return;

            animator.SetBool(humanAnimatorParameters.IsStrafing, isStrafing); ;
            animator.SetBool(humanAnimatorParameters.IsRunning, isSprinting);
            animator.SetBool(humanAnimatorParameters.IsGrounded, isGrounded);
            animator.SetFloat(humanAnimatorParameters.GroundDistance, groundDistance);

            if (isStrafing)
            {
                animator.SetFloat(humanAnimatorParameters.InputHorizontal, stopMove ? 0 : horizontalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
                animator.SetFloat(humanAnimatorParameters.InputVertical, stopMove ? 0 : verticalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
            }
            else
            {
                animator.SetFloat(humanAnimatorParameters.InputVertical, stopMove ? 0 : verticalSpeed, freeSpeed.animationSmooth, Time.deltaTime);
            }

            animator.SetFloat(humanAnimatorParameters.InputMagnitude, stopMove ? 0f : inputMagnitude, isStrafing ? strafeSpeed.animationSmooth : freeSpeed.animationSmooth, Time.deltaTime);
        }
    }
    
    public static class humanAnimatorParameters
    {
        public static int InputHorizontal = Animator.StringToHash("InputHorizontal");
        public static int InputVertical = Animator.StringToHash("InputVertical");
        public static int InputMagnitude = Animator.StringToHash("InputMagnitude");
        public static int IsGrounded = Animator.StringToHash("IsGrounded");
        public static int IsRunning = Animator.StringToHash("IsRunning");
        public static int GroundDistance = Animator.StringToHash("GroundDistance");
    }
    
    
}