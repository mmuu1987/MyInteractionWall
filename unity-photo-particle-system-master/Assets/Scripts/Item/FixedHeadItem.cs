using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 卓越风采二级界面的固定头像的脚本
/// </summary>
public class FixedHeadItem : MonoBehaviour
{

    private string headName;

    private Image headSprite;

    private PersonInfo _info;

    private void Awake()
    {
        headSprite = this.GetComponent<Image>();
    }

    public void SetData(PersonInfo info)
    {

        _info = info;
        if (headSprite == null) headSprite = this.GetComponent<Image>();
        headSprite.sprite = _info.headTex;
        headName = _info.PersonName;
    }
}
