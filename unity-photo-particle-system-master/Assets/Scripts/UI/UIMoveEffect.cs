using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using XHFrameWork;

/// <summary>
/// 按钮点击移动效果
/// </summary>
public class UIMoveEffect : MonoBehaviour {


    public List<Image>  Buttons = new List<Image>();

    public List<Sprite> ButtonOriginal  = new List<Sprite>();

    public List<Sprite> ButtonSelect= new List<Sprite>(); 

	// Use this for initialization
	void Start () {

        //if (Buttons.Count != ButtonOriginal.Count && ButtonOriginal.Count != ButtonSelect.Count)
        //{
        //   // throw new UnityException("三个数组的元素要一致");
        //}

        //foreach (Image button in Buttons)
        //{
        //    EventTriggerListener.Get(button.gameObject).SetEventHandle(EnumTouchEventType.OnClick, OnClick);
        //}
        //SetIndex(1);  
	}

    private void OnClick(GameObject _listener, object _args, params object[] _params)
    {
        string str = _listener.name;

        str = str.Substring(0, 1);//拿到索引   

        int index = int.Parse(str);

        for (int i = 0; i < Buttons.Count; i++)
        {
            if (i >= index)
            {
                RectTransform rtf = Buttons[i].rectTransform;
                rtf.DOAnchorPosX(rtf.position.x + 10, 0.5f);
            }
        }
    }

    public void SetIndex(int index)
    {
        for (int i = 0; i < Buttons.Count; i++)
        {
            if (i >= index)
            {
                RectTransform rtf = Buttons[i].rectTransform;
                rtf.DOAnchorPosX(rtf.position.x, 0.5f);
            }
        }
    }
	// Update is called once per frame
	void Update () {
		
	}

    public void Select()
    {
        
    }
}
