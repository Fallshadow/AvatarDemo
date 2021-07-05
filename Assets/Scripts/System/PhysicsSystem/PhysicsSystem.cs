using UnityEngine;

namespace ASeKi.system
{
    [RequireComponent(typeof(Rigidbody),typeof(CapsuleCollider))]
    public class PhysicsSystem : MonoBehaviour, ISystem
    {
        private Rigidbody _rigidbody;                                                      // 刚体组件
        private PhysicMaterial frictionPhysics, maxFrictionPhysics, slippyPhysics;         // 物理材质
        private CapsuleCollider _capsuleCollider;                                          // 胶囊碰撞

        
        public void Init()
        {
            // 在墙边缘滑动角色时角色的物理材质
            frictionPhysics = new PhysicMaterial();
            frictionPhysics.name = "frictionPhysics";
            frictionPhysics.staticFriction = .25f;
            frictionPhysics.dynamicFriction = .25f;
            frictionPhysics.frictionCombine = PhysicMaterialCombine.Multiply;

            // 防止碰撞器在斜坡上打滑
            maxFrictionPhysics = new PhysicMaterial();
            maxFrictionPhysics.name = "maxFrictionPhysics";
            maxFrictionPhysics.staticFriction = 1f;
            maxFrictionPhysics.dynamicFriction = 1f;
            maxFrictionPhysics.frictionCombine = PhysicMaterialCombine.Maximum;

            // 空中碰撞
            slippyPhysics = new PhysicMaterial();
            slippyPhysics.name = "slippyPhysics";
            slippyPhysics.staticFriction = 0f;
            slippyPhysics.dynamicFriction = 0f;
            slippyPhysics.frictionCombine = PhysicMaterialCombine.Minimum;

            _rigidbody = GetComponent<Rigidbody>();
            
            initCapsuleCollider();
            
            isGrounded = true;
        }

        #region CapsuleCollider 关于胶囊碰撞的信息

        private float colliderHeight, colliderRadius;
        private Vector3 colliderCenter;

        protected virtual void initCapsuleCollider()
        {
            _capsuleCollider = GetComponent<CapsuleCollider>();
            // 保存胶囊碰撞体信息
            colliderCenter = _capsuleCollider.center;
            colliderRadius = _capsuleCollider.radius;
            colliderHeight = _capsuleCollider.height;
        }

        #endregion
        
        #region Ground 关于地面检测的信息
        
        private bool isGrounded = true;
        private RaycastHit groundHit;                      // 射到地面的射线信息
        private LayerMask groundLayer = 1 << 0;
        private float groundMinDistance = 0.25f;
        private float groundMaxDistance = 0.5f;
        private float groundDistance;
        
        protected virtual void CheckGround()
        {
            checkGroundDistance();
            ControlMaterialPhysics();

            if (groundDistance <= groundMinDistance)
            {
                isGrounded = true;
                if (!isJumping && groundDistance > 0.05f)
                    _rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);

                heightReached = transform.position.y;
            }
            else
            {
                if (groundDistance >= groundMaxDistance)
                {
                    // set IsGrounded to false 
                    isGrounded = false;
                    // check vertical velocity
                    verticalVelocity = _rigidbody.velocity.y;
                    // apply extra gravity when falling
                    if (!isJumping)
                    {
                        _rigidbody.AddForce(transform.up * extraGravity * Time.deltaTime, ForceMode.VelocityChange);
                    }
                }
                else if (!isJumping)
                {
                    _rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);
                }
            }
        }
        
        // 检查地面距离
        protected virtual void checkGroundDistance()
        {
            if (_capsuleCollider != null)
            {
                float radius = _capsuleCollider.radius * 0.9f;
                var dist = 10f;

                // 从自身位置中心向下发射射线
                Ray ray2 = new Ray(transform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);

                // 检查10米外的碰撞 计算距离
                if (Physics.Raycast(ray2, out groundHit, (colliderHeight / 2) + dist, groundLayer) && !groundHit.collider.isTrigger)
                    dist = transform.position.y - groundHit.point.y;
                // 距离超出时还要检测胶囊下球面是否在地面上
                if (dist >= groundMinDistance)
                {
                    Vector3 pos = transform.position + Vector3.up * (_capsuleCollider.radius);
                    Ray ray = new Ray(pos, -Vector3.up);
                    if (Physics.SphereCast(ray, radius, out groundHit, _capsuleCollider.radius + groundMaxDistance, groundLayer) && !groundHit.collider.isTrigger)
                    {
                        Physics.Linecast(groundHit.point + (Vector3.up * 0.1f), groundHit.point + Vector3.down * 0.15f, out groundHit, groundLayer);
                        float newDist = transform.position.y - groundHit.point.y;
                        if (dist > newDist) dist = newDist;
                    }
                }
                groundDistance = (float)System.Math.Round(dist, 2);
            }
        }
        
        // 计算地面接触法线与竖直方向夹角
        protected virtual float groundAngle()
        {
            var groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
            return groundAngle;
        }
        #endregion

        #region 关于物理材质切换
        
        [Range(30, 80)] public float slopeLimit = 75f;
        
        protected virtual void ControlMaterialPhysics()
        {
            // change the physics material to very slip when not grounded
            _capsuleCollider.material = (isGrounded && groundAngle() <= slopeLimit + 1) ? frictionPhysics : slippyPhysics;

            if (isGrounded && input == Vector3.zero)
                _capsuleCollider.material = maxFrictionPhysics;
            else if (isGrounded && input != Vector3.zero)
                _capsuleCollider.material = frictionPhysics;
            else
                _capsuleCollider.material = slippyPhysics;
        }

        #endregion
    }
}