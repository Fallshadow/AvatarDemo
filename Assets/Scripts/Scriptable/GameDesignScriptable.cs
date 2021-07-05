using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDesignScriptable", menuName = "ScriptableObject/GameDesignScriptable")]
public class GameDesignScriptable : ScriptableObject
{
    public EntityEnum curEntityType = EntityEnum.EE_HUMAN;
}
