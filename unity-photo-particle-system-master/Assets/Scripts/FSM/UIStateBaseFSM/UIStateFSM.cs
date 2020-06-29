using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XHFrameWork;

public class UIStateFSM : FsmState<UIControl>
{
    /// <summary>
    /// UI的根物体
    /// </summary>
    public Transform Parent;

   

    protected List<EventTriggerListener> EventTriggers;

    public UIStateFSM(Transform go)
    {

        EventTriggers = new List<EventTriggerListener>();
        if (go != null)
        {
            Parent = go;
        }
        
    }

    public override void Enter()
    {
        base.Enter();
        //进入状态的时候，这个状态的按钮激活 
        foreach (EventTriggerListener trigger in EventTriggers)
        {
            trigger.enabled = true;
        }
        if (Parent != null)
            Parent.gameObject.SetActive(true);
    }

    public override void Exit()
    {
        base.Exit();
        //进入状态的时候，这个状态的按钮不激活
        foreach (EventTriggerListener trigger in EventTriggers)
        {
            trigger.enabled = false;
        }
        if(Parent!=null)
        Parent.gameObject.SetActive(false);
    }
}
