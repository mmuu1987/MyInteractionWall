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

        List<PersonInfo> infos = PictureHandle.Instance.HonorWall;
      

        for (int i = 0; i < images.Count; i++)
        {
            int index = i % infos.Count;

            Sprite temp = infos[index].headTex;

            if (temp == null)
            {
                Debug.Log(index);
            }

            images[i].sprite = infos[index].headTex;

            if(images[i].sprite !=null)
            images[i].name = images[i].sprite.name;
            else
            {
                Debug.Log(infos[index].PersonName);
            }
        }
        //保存索引
        Common.PictureIndex = images.Count;
    }

    private void ClickEvent(int index, PointerEventData data,Vector3 targetPos)
    {
        //Debug.Log("点击的索引是  " + index + "position is " + data.position);

        HeadItem item = Instantiate(HonorItem, Canvas.transform);

        item.transform.SetSiblingIndex(10);

        item.RectTransform.anchoredPosition = data.position;

        //Vector3 targetPos = data.position;

        //if (targetPos.x < 3840f) targetPos.x += 350f;
        //if (targetPos.x > 3840f) targetPos.x -= 350f;
        //if (targetPos.y < 1600f) targetPos.y = 1600;
        //if (targetPos.y > 3240) targetPos.y = 3240;

        targetPos.y = 3200f;
        item.RectTransform.DOAnchorPos(targetPos, 0.5f).SetEase(Ease.InOutQuad);


        item.RectTransform.DOScale(Vector3.one*2, 0.5f).SetEase(Ease.InOutQuad);

        //加载内容

        foreach (PersonInfo personInfo in PictureHandle.Instance.HonorWall)
        {
            if (personInfo.PictureIndex == index)
            {
                item.SetData(personInfo);
                break;
            }
        }


    }
}
