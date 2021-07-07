using UnityEngine;

namespace ASeKi.system
{
    [RequireComponent(typeof(Rigidbody),typeof(CapsuleCollider))]
    public class PhysicsSystem : MonoBehaviour, ISystem
    {
        private Rigidbody _rigidbody;                                                      // 刚体组件
        private PhysicMaterial frictionPhysics, maxFrictionPhysics, slippyPhysics;         // 物理材质
        private CapsuleCollider _capsuleCollider;                                          // 胶囊碰撞
        private Vector3 inputVector3Param;                                                 // 外界输入参数
        private Vector3 inputSmooth;                                                       // 将外界参数转换为平滑的实际应用参数
        private Vector3 moveDirection;                                                     // 移动方向
        
        public void InitSystem()
        {
            initPhysicMaterial();

            _rigidbody = GetComponent<Rigidbody>();
            
            initCapsuleCollider();
            
            isGrounded = true;
        }

        public void UpdateSystem()
        {
            ControlGround();
            
            ControlJump();
            
            ControlAirMove();
            
            ControlMaterialPhysics();
        }

        public void FixedUpdateSystem()
        {
            
        }

        // 外界参数传入，管理者最好是先调用设置参数然后再调用此系统的更新循环
        public void SetParam(Vector3 input,Vector3 moveDirectionP, LayerMask groundLayerP, bool isJumpingP)
        {
            inputVector3Param = input;
            moveDirection = moveDirectionP;
            groundLayer = groundLayerP;
            if (isJumping)
            {
                return;
            }
            isJumping = isJumpingP;
            
        }
        
        #region 关于胶囊碰撞的信息

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
        
        #region 关于地面检测的信息
        
        public bool isGrounded = true;
        private RaycastHit groundHit;                      // 射到地面的射线信息
        private LayerMask groundLayer;
        private float groundMinDistance = 0.25f;           // 小于这个距离被认为是在地面
        private float groundMaxDistance = 0.5f;            // 大于这个距离被认为不是在地面
        private float groundDistance;                      // 地面距离
        
        private float extraGravity = -10f;                  // 角色离地面有一定距离，而且并非处于起跳状态时，给予一定向下的力
        
        // 这边在下落的时候会加入额外的力去让它快速下落
        protected virtual void ControlGround()
        {
            checkGroundDistance();

            if (groundDistance <= groundMinDistance)
            {
                isGrounded = true;
                if (!isJumping && groundDistance > 0.05f)
                    _rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);
            }
            else
            {
                if (groundDistance >= groundMaxDistance)
                {
                    isGrounded = false;
                    if (!isJumping)
                    {
                        _rigidbody.AddForce(Time.deltaTime * extraGravity * transform.up, ForceMode.VelocityChange);
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
        public virtual float GroundAngle()
        {
            var groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
            return groundAngle;
        }
        #endregion

        #region 关于物理材质切换
        
        [Range(30, 80)] public float slopeLimit = 75f;

        protected virtual void initPhysicMaterial()
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
        }
        
        protected virtual void ControlMaterialPhysics()
        {
            // 根据是否在地面、斜坡 改变胶囊体材质
            _capsuleCollider.material = (isGrounded && GroundAngle() <= slopeLimit + 1) ? frictionPhysics : slippyPhysics;
            
            // 根据输入改变材质
            if (isGrounded && inputVector3Param == Vector3.zero)
                _capsuleCollider.material = maxFrictionPhysics;
            else if (isGrounded && inputVector3Param != Vector3.zero)
                _capsuleCollider.material = frictionPhysics;
            else
                _capsuleCollider.material = slippyPhysics;
        }

        #endregion

        #region 关于跳跃

        public bool  isJumping = false;
        private float jumpCounter = 0.3f;
        private float jumpHeight = 4f;
        private float airSpeed = 5f;
        private float airSmooth = 6f;                    // 空中平滑速率 * DetalTime
        private bool jumpWithRigidbodyForce = false;     // 使用刚体速度来影响跳跃高度 开关
        
        protected virtual void ControlJump()
        {
            if (!isJumping) return;
            // 被认为是跳跃中的时间只有这些
            jumpCounter -= Time.deltaTime;
            if (jumpCounter <= 0)
            {
                jumpCounter = 0;
                isJumping = false;
            }
            // TODO:是不是应该增加而不是修改 
            var vel = _rigidbody.velocity;
            vel.y = jumpHeight;
            _rigidbody.velocity = vel;
        }
        
        public virtual void ControlAirMove()
        {
            if (isGrounded && !isJumping) return;
            inputSmooth = Vector3.Lerp(inputSmooth, inputVector3Param, airSmooth * Time.deltaTime);

            if (jumpWithRigidbodyForce && !isGrounded)
            {
                _rigidbody.AddForce(Time.deltaTime * airSpeed * moveDirection, ForceMode.VelocityChange);
                return;
            }

            moveDirection.y = 0;
            moveDirection.x = Mathf.Clamp(moveDirection.x, -1f, 1f);
            moveDirection.z = Mathf.Clamp(moveDirection.z, -1f, 1f);

            Vector3 targetPosition = _rigidbody.position + Time.deltaTime * airSpeed * moveDirection;
            Vector3 targetVelocity = (targetPosition - transform.position) / Time.deltaTime;

            targetVelocity.y = _rigidbody.velocity.y;
            _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, targetVelocity, airSmooth * Time.deltaTime);
        }

        #endregion



        #region 外界接口
        public virtual CapsuleCollider GetCapsuleCollider()
        {
            return _capsuleCollider;
        }
        public virtual Vector3 GetRigidbodyVelocity()
        {
            return _rigidbody.velocity;
        }
        
        public virtual Vector3 GetRigidbodyPosition()
        {
            return _rigidbody.position;
        }
        
        public virtual void SetRigidbodyVelocity(Vector3 veloctiy)
        {
            _rigidbody.velocity = veloctiy;
        }

        public virtual float GetGroundDistance()
        {
            return groundDistance;
        }
        #endregion
    }
}