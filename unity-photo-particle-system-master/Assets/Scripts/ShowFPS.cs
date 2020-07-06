using UnityEngine;
using System.Collections;

public class ShowFPS : MonoBehaviour {
	/// <summary>
	/// 每次刷新计算的时间      帧/秒
	/// </summary>
	public float updateInterval = 0.5f;
	/// <summary>
	/// 最后间隔结束时间
	/// </summary>
	private double lastInterval;
	private int frames = 0;
    private float currFPS;

    public bool IsShowFps = true;

    public string  CurrFPS
    {
        get { return currFPS.ToString("f2"); }
       
    }

    public float CurrentFPS
    {
        get { return currFPS; }
    }
    public static ShowFPS Instance;
    private void Awake()
    {
        //Debug.Log("隐藏也运行");
        Instance = this;
    }
	
	// Use this for initialization
	void Start () {
		lastInterval = Time.realtimeSinceStartup;
		frames = 0;
        //Application.targetFrameRate = 120;
    }

    // Update is called once per frame
    void Update () {
		
		++frames;
		float timeNow = Time.realtimeSinceStartup;
		if (timeNow > lastInterval + updateInterval)
		{
			currFPS = (float)(frames / (timeNow - lastInterval));
			frames = 0;
			lastInterval = timeNow;
		}
	}

    private GUIStyle fontStyle;


    private void OnGUI()
    {
        if (fontStyle == null)
        {
            fontStyle = new GUIStyle();

            fontStyle.normal.background = null;    //设置背景填充
            fontStyle.normal.textColor = new Color(1, 0, 0);   //设置字体颜色
            fontStyle.fontSize = 200;       //字体大小


        }
        if (IsShowFps)
            GUILayout.Label("FPS:" + currFPS.ToString("f2"), fontStyle);

    }
	
}
