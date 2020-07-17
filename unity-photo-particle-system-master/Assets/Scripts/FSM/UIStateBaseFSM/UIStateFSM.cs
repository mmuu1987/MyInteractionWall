using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using XHFrameWork;

public class UIStateFSM : FsmState<UIControl>
{
    /// <summary>
    /// UI的根物体
    /// </summary>
    public Transform Parent;

   

    protected List<EventTriggerListener> EventTriggers;


    protected RawImage _videoImage;


    protected VideoPlayer _videoPlayer;

    public UIStateFSM(Transform go)
    {

        EventTriggers = new List<EventTriggerListener>();
        if (go != null)
        {
            Parent = go;
        }


        if (Parent != null)
        {
            _videoImage = Parent.parent.transform.Find("VideoPlay").GetComponent<RawImage>();

            _videoPlayer = _videoImage.GetComponent<VideoPlayer>();

            RenderTexture rt = new RenderTexture(1280, 720, 0);

            _videoPlayer.renderMode = VideoRenderMode.RenderTexture;

            _videoPlayer.targetTexture = rt;

            _videoImage.texture = rt;
        }
       
    }

    public override void Enter()
    {
        base.Enter();
        //进入状态的时候，这个状态的按钮激活 
        foreach (EventTriggerListener trigger in EventTriggers)
        {
            trigger.enabled = true;
        }
        if (Parent != null)
        {
            Parent.gameObject.SetActive(true);
            _videoImage.rectTransform.localScale = Vector3.zero;
        }

       
    }
    /// <summary>
    /// 检测是否是视频贴图，如果是，则播放视频
    /// </summary>
    protected  void CheckVideoTex(Texture2D tex,GameObject showGo)
    {

        if (tex != null && tex.name.Contains("video__________"))
        {
            string[] strs = { "__________" };
            string[] path = tex.name.Split(strs, StringSplitOptions.None);
            showGo.gameObject.SetActive(false);

            string url = path[1];

            _videoPlayer.url = "file://" + url;
            _videoPlayer.transform.localScale = Vector3.one * 0.35f;
            _videoPlayer.enabled = true;
            _videoPlayer.Play();
        }
        else
        {
            _videoPlayer.transform.localScale = Vector3.zero;
            showGo.gameObject.SetActive(true);
            _videoPlayer.Stop();
          

        }
    }
    public void AddVideoTex(List<Texture2D> texs, List<string> videoPaths)
    {
        Texture2D tex = Resources.Load<Texture2D>("暂停");

        foreach (string videoPath in videoPaths)
        {

            FileInfo fileInfo = new FileInfo(videoPath);
            if (!string.IsNullOrEmpty(videoPath))
            {
                //加载视频贴图
                Texture2D newtex = UnityEngine.Object.Instantiate(tex);
                newtex.name = "video__________" + fileInfo.FullName;//"__________"为分隔符
                texs.Add(newtex);

            }
        }
    }
    public override void Exit()
    {
        base.Exit();
        //进入状态的时候，这个状态的按钮不激活
        foreach (EventTriggerListener trigger in EventTriggers)
        {
            trigger.enabled = false;
        }
        if(Parent!=null)
        Parent.gameObject.SetActive(false);
    }
}
