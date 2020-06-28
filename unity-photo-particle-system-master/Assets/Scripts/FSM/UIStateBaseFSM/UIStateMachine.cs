using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStateMachine : FsmStateMachine<UIControl>
{
    public UIStateMachine(UIControl owner)
        : base(owner)
    {

    }
}
