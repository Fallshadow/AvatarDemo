using System;
using ASeKi.input;
using ASeKi.system;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public AttrBase AttrValue;

    [Header("System")] 
    [SerializeField] private HumanAnimatorSystem animatorSystem;
    [SerializeField] private PhysicsSystem physicsSystem;
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private CameraSystem cameraSystem;


#if UNITY_EDITOR
    [Header("Data")]
    public GameDesignScriptable gameDesignSetting;
#endif

    protected virtual void Awake()
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
        

        cameraSystem.SetParam(inputSystem.InputVector3Param);
        cameraSystem.UpdateSystem();
        animatorSystem.SetParam(new HumanAnimatorParametersStruct(isRun, physicsSystem.isGrounded, stopMove,
            physicsSystem.GetGroundDistance(), cameraSystem.GetMoveDirection(), AttrValue.runSpeed, AttrValue.walkSpeed));
        animatorSystem.UpdateSystem();
        if (inputSystem.IsJumping)
        {
            inputSystem.IsJumping = false;
            if (JumpConditions())
            {
                needJump = true;
            }
        }
    }

    private void FixedUpdate()
    {
        physicsSystem.SetParam(inputSystem.InputVector3Param, cameraSystem.GetMoveDirection(), AttrValue.groundLayer, needJump);
        if (needJump)
        {
            JumpAnim();
        }
        physicsSystem.FixedUpdateSystem();
        CheckSlopeLimit();
        Rotation();
        Run(inputSystem.IsRunning);
        Move(cameraSystem.GetMoveDirection());

    }
    
    public virtual void JumpAnim()
    {
        if (inputSystem.InputVector3Param.sqrMagnitude < 0.1f)
            animatorSystem.animator.CrossFadeInFixedTime("Jump", 0.1f);
        else
            animatorSystem.animator.CrossFadeInFixedTime("JumpMove", .2f);
    }


    protected virtual void CheckGround()
    {
    }

    #region ????????????

    private float moveSpeed; // ????????????Move????????????????????????

    // ????????????----?????????
    // ??????????????????????????????  RootMotion??????  ?????????????????????  ?????????
    // ??????????????? ??????
    // ???????????????????????????/???????????? ??????????????????????????? ????????????????????????
    public virtual void Move(Vector3 _direction)
    {
        if (!physicsSystem.isGrounded || physicsSystem.isJumping) return;
        Vector3 _rigidbodyPos = physicsSystem.GetRigidbodyPosition();
        Vector3 _rigidbodyVel = physicsSystem.GetRigidbodyVelocity();
        moveSpeed = Mathf.Lerp(moveSpeed, isRun ? AttrValue.runSpeed : AttrValue.walkSpeed, AttrValue.moveSmoth * Time.deltaTime);

        _direction.y = 0;
        _direction.x = Mathf.Clamp(_direction.x, -1f, 1f);
        _direction.z = Mathf.Clamp(_direction.z, -1f, 1f);

        // ?????????????????????
        if (_direction.magnitude > 1f)
        {
            _direction.Normalize();
        }

        Vector3 targetPosition = _rigidbodyPos + Time.deltaTime * (stopMove ? 0 : moveSpeed) * _direction;
        Vector3 targetVelocity = (targetPosition - transform.position) / Time.deltaTime;

        targetVelocity.y = _rigidbodyVel.y;
        physicsSystem.SetRigidbodyVelocity(targetVelocity);
    }

    #endregion

    #region ??????????????????????????????????????????

    bool stopMove = false;

    // ????????????----?????????
    // ??????????????????????????????  ????????????  ???????????????????????????????????????  ?????????
    // ??????????????? ??????
    // ???????????????????????????/???????????? ??????????????????????????? ????????????????????????
    public virtual void CheckSlopeLimit()
    {
        Vector3 inputVector3 = inputSystem.GetInputVector3();
        Vector3 moveDirection = cameraSystem.GetMoveDirection();
        CapsuleCollider _capsuleCollider = physicsSystem.GetCapsuleCollider();

        // ??????????????????
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

    #region ????????????

    private bool isRun; // ?????????????????????

    public virtual void Run(bool value)
    {
        var sprintConditions = inputSystem.InputVector3Param.sqrMagnitude > 0.1f && physicsSystem.isGrounded;

        if (value && sprintConditions)
        {
            isRun = true;
        }
        else
        {
            isRun = false;
        }
    }

    #endregion

    #region ????????????

    bool needJump = false;

    protected virtual bool JumpConditions()
    {
        return physicsSystem.isGrounded && physicsSystem.GroundAngle() < AttrValue.slopeLimit && !physicsSystem.isJumping && !stopMove;
    }

    #endregion

    #region ????????????
    
    public virtual void Rotation()
    {
        if (!physicsSystem.isGrounded) return;
        if (inputSystem.InputVector3Param == Vector3.zero) return;
        Vector3 dir = cameraSystem.GetMoveDirection();
        dir.y = 0f;
        Vector3 desiredForward = Vector3.RotateTowards(animatorSystem.transform.forward, dir.normalized, AttrValue.rotateSpeed * Time.deltaTime, .1f);
        Quaternion _newRotation = Quaternion.LookRotation(desiredForward);
        animatorSystem.transform.rotation = _newRotation;
    }

    #endregion
}