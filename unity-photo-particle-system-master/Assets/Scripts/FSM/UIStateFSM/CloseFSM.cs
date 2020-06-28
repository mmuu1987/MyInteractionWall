using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseFSM : UIStateFSM {

	
    public CloseFSM(Transform go) : base(go)
    {

    }

    public override void Enter()
    {
        base.Enter();

        Target.transform.Find("CompanyIntroduction").gameObject.SetActive(false);
    }
}
