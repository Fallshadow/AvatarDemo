using System;
using ASeKi.input;
using ASeKi.system;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public AttrBase AttrValue;

    [Header("System")] [SerializeField] private HumanAnimatorSystem animatorSystem;
    [SerializeField] private PhysicsSystem physicsSystem;
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private CameraSystem cameraSystem;

    [Header("Data")]
#if UNITY_EDITOR
    public GameDesignScriptable gameDesignSetting;
#endif

    private void Awake()
    {
        inputSystem = GetComponentInChildren<InputSystem>();
        inputSystem.InitSystem();
        physicsSystem = GetComponentInChildren<PhysicsSystem>();
        physicsSystem.InitSystem();
        animatorSystem = GetComponentInChildren<HumanAnimatorSystem>();
        animatorSystem.InitSystem();
        cameraSystem = GetComponentInChildren<CameraSystem>();
        cameraSystem.InitSystem();
        initData();
    }

    private void initData()
    {
        AttrValue.groundLayer = 1;
        AttrValue.slopeLimit = 75f;
        AttrValue.moveSmoth = 6f;
        AttrValue.walkSpeed = 2f;
        AttrValue.runSpeed = 4f;
        AttrValue.rotateSpeed = 16f;
        AttrValue.rotateSmooth = 6f;
    }

    private void OnEnable()
    {
    }

    private void Update()
    {
        needJump = false;
        inputSystem.UpdateSystem();
        if (inputSystem.IsJumping)
        {
            inputSystem.IsJumping = false;
            if (JumpConditions())
            {
                needJump = true;
            }
        }

        cameraSystem.SetParam(inputSystem.InputVector3Param);
        cameraSystem.UpdateSystem();
        animatorSystem.SetParam(new HumanAnimatorParametersStruct(isRun, physicsSystem.isGrounded, stopMove,
            physicsSystem.GetGroundDistance(), cameraSystem.GetMoveDirection(), AttrValue.runSpeed, AttrValue.walkSpeed));
        animatorSystem.UpdateSystem();
    }

    private void FixedUpdate()
    {
        physicsSystem.SetParam(inputSystem.InputVector3Param, cameraSystem.GetMoveDirection(), AttrValue.groundLayer, needJump);
        physicsSystem.UpdateSystem();

        Rotation();
        Run(inputSystem.IsRunning);
        if (!animatorSystem.GetRootMotionSwitch())
        {
            Move(cameraSystem.GetMoveDirection());
        }

    }

    public virtual void OnAnimatorMove()
    {
        if (animatorSystem.ControlAnimatorRootMotion(inputSystem.InputVector3Param))
        {
            Move(cameraSystem.GetMoveDirection());
        }
    }

    protected virtual void CheckGround()
    {
    }

    #region 移动行为

    private float moveSpeed; // 专们供给Move移动的暂存缓冲值

    // 移动功能----普通版
    // 使用到的系统的信息：  RootMotion开关  物理系统的刚体  输入量
    // 启动信息： 方向
    // 状态信息：不在地面/跳越过程 不会进入这里的移动 之后用状态机包装
    public virtual void Move(Vector3 _direction)
    {
        if (!physicsSystem.isGrounded || physicsSystem.isJumping) return;
        Vector3 _rigidbodyPos = physicsSystem.GetRigidbodyPosition();
        Vector3 _rigidbodyVel = physicsSystem.GetRigidbodyVelocity();
        Vector3 inputVector3 = inputSystem.GetInputVector3();
        bool animatorSwitch = animatorSystem.GetRootMotionSwitch();
        moveSpeed = Mathf.Lerp(moveSpeed, isRun ? AttrValue.runSpeed : AttrValue.walkSpeed, AttrValue.moveSmoth * Time.deltaTime);

        _direction.y = 0;
        _direction.x = Mathf.Clamp(_direction.x, -1f, 1f);
        _direction.z = Mathf.Clamp(_direction.z, -1f, 1f);

        // 格式化方向向量
        if (_direction.magnitude > 1f)
        {
            _direction.Normalize();
        }

        Vector3 targetPosition = (animatorSwitch ? animatorSystem.GetAnimationPos() : _rigidbodyPos) + Time.deltaTime * (stopMove ? 0 : AttrValue.walkSpeed) * _direction;
        Vector3 targetVelocity = (targetPosition - transform.position) / Time.deltaTime;

        targetVelocity.y = _rigidbodyVel.y;
        Debug.Log($"[Entity] 刚体速度{targetVelocity}");
        physicsSystem.SetRigidbodyVelocity(targetVelocity);
    }

    #endregion

    #region 斜坡大于一定角度停止移动行为

    bool stopMove = false;

    // 斜坡策略----普通版
    // 使用到的系统的信息：  移动方向  物理系统的胶囊体高度、半径  输入量
    // 启动信息： 方向
    // 状态信息：不在地面/跳越过程 不会进入这里的移动 之后用状态机包装
    public virtual void CheckSlopeLimit()
    {
        Vector3 inputVector3 = inputSystem.GetInputVector3();
        Vector3 moveDirection = cameraSystem.GetMoveDirection();
        CapsuleCollider _capsuleCollider = physicsSystem.GetCapsuleCollider();

        // 视为没有输入
        if (inputVector3.sqrMagnitude < 0.1) return;

        RaycastHit hitinfo;
        float hitAngle = 0f;

        if (Physics.Linecast(transform.position + Vector3.up * (_capsuleCollider.height * 0.5f),
            transform.position + moveDirection.normalized * (_capsuleCollider.radius + 0.2f),
            out hitinfo, AttrValue.groundLayer))
        {
            hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

            var targetPoint = hitinfo.point + moveDirection.normalized * _capsuleCollider.radius;
            if ((hitAngle > AttrValue.slopeLimit) && Physics.Linecast(transform.position + Vector3.up * (_capsuleCollider.height * 0.5f), targetPoint, out hitinfo, AttrValue.groundLayer))
            {
                hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

                if (hitAngle > AttrValue.slopeLimit && hitAngle < 85f)
                {
                    stopMove = true;
                    return;
                }
            }
        }

        stopMove = false;
    }

    #endregion

    #region 跑步行为

    private bool isRun; // 标志是否在冲刺

    public virtual void Run(bool value)
    {
        var sprintConditions = inputSystem.InputVector3Param.sqrMagnitude > 0.1f && physicsSystem.isGrounded;

        if (value && sprintConditions)
        {
            if (inputSystem.InputVector3Param.sqrMagnitude > 0.1f)
            {
                if (physicsSystem.isGrounded)
                {
                    isRun = !isRun;
                }
                else if (!isRun)
                {
                    isRun = true;
                }
            }
            else if (isRun)
            {
                isRun = false;
            }
        }
        else if (isRun)
        {
            isRun = false;
        }
    }

    #endregion

    #region 跳跃行为

    bool needJump = false;

    protected virtual bool JumpConditions()
    {
        return physicsSystem.isGrounded && physicsSystem.GroundAngle() < AttrValue.slopeLimit && !physicsSystem.isJumping && !stopMove;
    }

    #endregion

    #region 旋转行为

    private bool lockRotation = false;
    private Vector3 rotateSmooth;
    public bool jumpAndRotate = true;

    public virtual void Rotation()
    {
        if (lockRotation) return;

        bool validInput = inputSystem.InputVector3Param != Vector3.zero;

        if (validInput)
        {
            // calculate input smooth
            rotateSmooth = Vector3.Lerp(rotateSmooth, inputSystem.InputVector3Param, AttrValue.rotateSmooth * Time.deltaTime);

            Vector3 dir = cameraSystem.GetMoveDirection();
            RotateToDirection(dir, AttrValue.rotateSpeed);
        }
    }

    public virtual void RotateToDirection(Vector3 direction, float rotationSpeed)
    {
        if (!jumpAndRotate && !physicsSystem.isGrounded) return;
        direction.y = 0f;
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, direction.normalized, rotationSpeed * Time.deltaTime, .1f);
        Quaternion _newRotation = Quaternion.LookRotation(desiredForward);
        transform.rotation = _newRotation;
    }

    #endregion
}