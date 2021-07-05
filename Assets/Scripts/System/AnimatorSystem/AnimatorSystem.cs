using UnityEngine;

namespace ASeKi.system
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorSystem : MonoBehaviour, ISystem
    {
        private Animator animator;


        public void Init()
        {
            animator = GetComponent<Animator>();
            animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
        }
    }
    

}