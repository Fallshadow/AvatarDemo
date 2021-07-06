using UnityEngine;

namespace ASeKi.system
{
    public class CameraSystem : MonoBehaviour, ISystem
    {
        private Vector3 moveDirection;                     // 角色移动方向
        private Vector3 inputVector3Param;                 // 外界输入参数
        private float directionSmooth;                     // 方向缓动数值
        public bool rotateByWorld = false;                 // 绕着世界坐标旋转/在相机空间旋转
        
        public void SetParam(Vector3 inputVector3ParamP)
        {
            inputVector3Param = inputVector3ParamP;
        }

        
        public void InitSystem()
        {
            
        }

        public void UpdateSystem()
        {
            UpdateMoveDirection();
        }

        public void FixedUpdateSystem()
        {
            
        }
        
        // 决定移动方向（根据相机空间或者根据世界坐标）
        public virtual void UpdateMoveDirection(Transform referenceTransform = null)
        {
            // 输入的长度小于0.01 视为不动
            if (inputVector3Param.magnitude <= 0.01)
            {
                moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, directionSmooth * Time.deltaTime);
                return;
            }

            if (referenceTransform && !rotateByWorld)
            {
                var right = referenceTransform.right;
                right.y = 0;
                var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
                moveDirection = inputVector3Param.x * right + inputVector3Param.z * forward;
            }
            else
            {
                moveDirection = new Vector3(inputVector3Param.x, 0, inputVector3Param.z);
            }
        }

        #region 外界接口

        // 获取人物移动方向
        public Vector3 GetMoveDirection()
        {
            return moveDirection;
        }

        #endregion
        
    }
}