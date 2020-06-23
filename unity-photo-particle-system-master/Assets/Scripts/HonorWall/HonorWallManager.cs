using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HonorWallManager : MonoBehaviour
{


    public HeadItem HonorItem;

    public Canvas Canvas;

    private void Start()
    {
        SetPersonInfo();
    }


    public void SetPersonInfo()
    {
        //获取所有的头像组件，该组件还没赋值头像
        HonorWallItem[] items = this.GetComponentsInChildren<HonorWallItem>();


        //绑定点击事件
        foreach (HonorWallItem item in items)
        {
            item.ClickEvent += ClickEvent;
        }

        List<Image> images = new List<Image>();

        foreach (HonorWallItem item in items)
        {
            images.AddRange(item.ButtonImages);
        }

        List<PersonInfo> infos = PictureHandle.Instance.PersonInfos;
      

        for (int i = 0; i < images.Count; i++)
        {
            int index = i % infos.Count;

            images[i].sprite = infos[index].headTex;

            images[i].name = images[i].sprite.name;

        }
    }

    private void ClickEvent(int index, PointerEventData data)
    {
        Debug.Log("点击的索引是  " + index + "position is " + data.position);

        HeadItem item = Instantiate(HonorItem, Canvas.transform);

        item.RectTransform.anchoredPosition = data.position;

        Vector3 targetPos = data.position;

        if (targetPos.x < 3840f) targetPos.x += 350f;
        if (targetPos.x > 3840f) targetPos.x -= 350f;
        if (targetPos.y < 1300f) targetPos.y = 1300f;
        if (targetPos.y > 2940f) targetPos.y = 2940f;

        item.RectTransform.DOAnchorPos(targetPos, 0.5f).SetEase(Ease.InOutQuad);
        item.RectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutQuad);

        //加载内容

        foreach (PersonInfo personInfo in PictureHandle.Instance.PersonInfos)
        {
            if (personInfo.PictureIndex == index)
            {
                item.SetData(personInfo);
                break;
            }
        }


    }
}
