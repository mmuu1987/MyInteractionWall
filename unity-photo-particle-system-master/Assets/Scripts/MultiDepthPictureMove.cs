using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 图片移动管理CPU版本
/// </summary>
public class MultiDepthPictureMove
{
    private List<PosAndDir> _datas;

    private DepthInfo [] _depthInfos;

    private List<PictureItem> items;

    private Transform _canvas;

    public MultiDepthPictureMove(PosAndDir [] datas,DepthInfo [] infos,Canvas canvas )
    {
        
        //触摸点最大为十个
        List<Vector3> clicks = new List<Vector3>();

        _canvas = canvas.transform;
        for (int i = 0; i < 10; i++)
        {
            clicks.Add(Vector3.one * 100000);
        }
      

        items = new List<PictureItem>();

        _datas = new List<PosAndDir>(datas);

        _depthInfos = infos;

        GameObject item = Resources.Load<GameObject>("Prefabs/PictureItem");

        Shader shader = Shader.Find("Unlit/MultiDepthCPU");

        Material mat = new Material(shader);

        foreach (PosAndDir data in datas)
        {
           if(data.moveTarget.y>=1000)continue;


            GameObject go = Object.Instantiate(item);

            Material newMat = Object.Instantiate(mat);

            PictureItem pitm = go.GetComponent<PictureItem>();

            pitm.SetData(data, _depthInfos, _canvas.transform, newMat);
            pitm.name = data.picIndex.ToString();
            items.Add(pitm);
        }


    }
    public void Enter()
    {
        //
    }
    private void CreatPicture()
    {

    }

    public void SetClickPoint(Vector3 clickPoint)
    {
        foreach (PictureItem pictureItem in items)
        {
            pictureItem.ClickPoint = clickPoint;
        }
    }
    public void Excute( Vector3[] clicks)
    {

        foreach (PictureItem pictureItem in items)
        {
            pictureItem.UpdateData( clicks);
        }
    }

    public void Exit()
    {
        
    }
}
