using ASeKi.system;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public AttrBase attrValue;
    private AnimatorSystem animatorSystem;
    private PhysicsSystem physicsSystem;


#if UNITY_EDITOR
    public GameDesignScriptable gameDesignSetting;
#endif
    
    public virtual void Init()
    {
        
    }

    protected virtual void CheckGround()
    {
        
    }
    
}
