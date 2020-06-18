using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 显示角色的详细内容和一些UI动画
/// </summary>
public class HeadItem : MonoBehaviour {



    private void Start()
    {
        DoAnimation();
    }

    public void DoAnimation()
    {
        this.GetComponent<RectTransform>().DOSizeDelta(new Vector2(405f, 1259f), 0.75f).SetEase(Ease.InCubic).SetDelay(1f);
        this.transform.Find("DownMotion").GetComponent<RectTransform>().DOSizeDelta(new Vector2(405f, 180f), 0.75f).SetEase(Ease.InCubic).SetDelay(1.2f);
    }
}
