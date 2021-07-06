using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttrBase
{
    public float Health;
    public float moveSmoth;                // 在地面自由移动时的lerp速度
    public LayerMask groundLayer;            // 墙的layer
    public float walkSpeed;
    public float runSpeed;
    public float rotateSpeed;            // 旋转速度
    public float rotateSmooth;            // 旋转速率
    
    public float slopeLimit;// 最大步行斜坡角度
}
