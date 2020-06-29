using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutstandingStyleFSM : UIStateFSM
{

    public Button HonorListBtn;

    public Button StandardList;

    public Button DoubleMillion;


    public List<Sprite> HonorTex;

    public List<Sprite> StandardTex;

    public List<Sprite> DoubleMillionTex;


    public OutstandingStyleFSM(Transform go) : base(go)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
