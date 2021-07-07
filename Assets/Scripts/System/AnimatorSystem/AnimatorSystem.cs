using UnityEngine;

namespace ASeKi.system
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorSystem : MonoBehaviour, ISystem
    {
        public Animator animator;
        public bool UseRootMotion = false;        // 是否使用RootMotion

        public virtual void InitSystem()
        {
            animator = GetComponent<Animator>();
            animator.updateMode = AnimatorUpdateMode.Normal;
        }

        public virtual void UpdateSystem()
        {
            
        }

        public virtual void FixedUpdateSystem()
        {
            
        }

        public bool ControlAnimatorRootMotion(Vector3 moveInput)
        {
            if (!enabled)
            {
                return false;
            }

            if (UseRootMotion)
            {
                if (moveInput == Vector3.zero)
                {
                    var transform1 = transform;
                    transform1.position = animator.rootPosition;
                    transform1.rotation = animator.rootRotation;
                }
            }
            
            return UseRootMotion;
        }
        


        #region 外界接口

        // 获取RootMotion开关
        public virtual bool GetRootMotionSwitch()
        {
            return UseRootMotion;
        }

        public virtual Vector3 GetAnimationPos()
        {
            return animator.rootPosition;
        }

        #endregion

    }
    

}