using ASeKi.fsm;

public class HumanEntity : Entity
{
    public readonly Fsm<HumanEntity> FSM = new Fsm<HumanEntity>();
    protected override void Awake()
    {
        base.Awake();
        FSM.Initialize(this);
        FSM.AddState((int) HumanEntityFsmState.Idle, new Idle());
        // FSM.AddState((int) HumanEntityFsmState.Run, new Reset());
        // FSM.AddState((int) HumanEntityFsmState.Jump, new EntryLogin());
        // FSM.AddState((int) HumanEntityFsmState.Walk, new EntryLogin());
    }

    private bool JudgeWalk()
    {
        
        return false;
    }
}