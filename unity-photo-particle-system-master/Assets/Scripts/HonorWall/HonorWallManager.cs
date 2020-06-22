using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HonorWallManager : MonoBehaviour {




    private void Start()
    {
        SetPersonInfo();
    }


    public void SetPersonInfo()
    {
        //获取所有的头像组件，该组件还没赋值头像
        HonorWallItem[] items = this.GetComponentsInChildren<HonorWallItem>();

        List<Image> images = new List<Image>();

        foreach (HonorWallItem item in items)
        {
            images.AddRange(item.ButtonImages);
        }

        List<Texture2D> texs = PictureHandle.Instance.PersonTexs;
        List<Sprite> sprites = new List<Sprite>();
       

        //把图片转成精灵  
        foreach (Texture2D texture2D in texs)
        {
            Sprite s = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
            sprites.Add(s);
        }

        for (int i = 0; i < images.Count; i++)
        {
            int index = i % sprites.Count;

            images[i].sprite = sprites[index];
            images[i].name = index.ToString();

        }
    }
}
