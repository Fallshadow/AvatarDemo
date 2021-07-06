using UnityEngine;

namespace ASeKi.system
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorSystem : MonoBehaviour, ISystem
    {
        private Animator animator;
        private bool useRootMotion = false;        // 是否使用RootMotion

        public void InitSystem()
        {
            animator = GetComponent<Animator>();
            animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
        }
        
        public void SetParam()
        {
            
        }

        public void UpdateSystem()
        {
            
        }

        public void FixedUpdateSystem()
        {
            
        }
        

        
        public bool ControlAnimatorRootMotion(Vector3 moveInput)
        {
            if (!enabled)
            {
                return false;
            }

            if (useRootMotion)
            {
                if (moveInput == Vector3.zero)
                {
                    var transform1 = transform;
                    transform1.position = animator.rootPosition;
                    transform1.rotation = animator.rootRotation;
                }
            }
            
            return useRootMotion;
        }
        


        #region 外界接口

        // 获取RootMotion开关
        public virtual bool GetRootMotionSwitch()
        {
            return useRootMotion;
        }

        public virtual Vector3 GetAnimationPos()
        {
            return animator.rootPosition;
        }

        #endregion

    }
    

}