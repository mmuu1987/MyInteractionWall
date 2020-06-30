using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XHFrameWork;

public class OutstandingStyleFSM : UIStateFSM
{

    public Button HonorListBtn;

    public Button StandardList;

    public Button DoubleMillion;


    public List<PersonInfo> HonorTex;

    public List<PersonInfo> StandardTex;

    public List<PersonInfo> DoubleMillionTex;

    private GameObject _fixedHeadGo;

    private Transform _ryGrid;

    private Transform _dbGrid;

    private Transform _sbwGrid;

    /// <summary>
    /// 显示内容的贴图  
    /// </summary>
    private RawImage ShowImage;


    private Transform _introduce;

    private Image _intrHead;

    private Text _intrName;

    private Text _intrDes;
    /// <summary>
    ///  
    /// </summary>
    /// <param name="go"></param>
    public OutstandingStyleFSM(Transform go) : base(go)
    {
        _ryGrid = Parent.Find("ry/Viewport/Content");

        _dbGrid = Parent.Find("db/Viewport/Content");

        _sbwGrid = Parent.Find("sbw/Viewport/Content");

        HonorListBtn = Parent.Find("1MDRT荣誉榜").GetComponent<Button>();

        StandardList = Parent.Find("2达标榜").GetComponent<Button>();

        DoubleMillion = Parent.Find("3双百万").GetComponent<Button>();

        _fixedHeadGo = Resources.Load<GameObject>("Prefabs/FixedHeadItem");

        HonorTex = PictureHandle.Instance.PersonInfos[0];

        StandardTex = PictureHandle.Instance.PersonInfos[1];

        DoubleMillionTex = PictureHandle.Instance.PersonInfos[2];

        ShowImage = Parent.parent.Find("ShowImage").GetComponent<RawImage>();

        _introduce = Parent.Find("introduce");

        _intrHead = Parent.Find("introduce/head").GetComponent<Image>();

        _intrName = Parent.Find("introduce/Name").GetComponent<Text>();

        _intrDes = Parent.Find("introduce/Text").GetComponent<Text>();


        LoadHeadInfo(_ryGrid, HonorTex);

        LoadHeadInfo(_dbGrid, StandardTex);

        LoadHeadInfo(_sbwGrid, DoubleMillionTex);

        HonorListBtn.onClick.AddListener((() =>
        {
            _ryGrid.parent.parent.gameObject.SetActive(true);
            _dbGrid.parent.parent.gameObject.SetActive(false);
            _sbwGrid.parent.parent.gameObject.SetActive(false);
        }));

        StandardList.onClick.AddListener((() =>
        {
            _ryGrid.parent.parent.gameObject.SetActive(false);
            _dbGrid.parent.parent.gameObject.SetActive(true);
            _sbwGrid.parent.parent.gameObject.SetActive(false);
        }));

        DoubleMillion.onClick.AddListener((() =>
        {
            _ryGrid.parent.parent.gameObject.SetActive(false);
            _dbGrid.parent.parent.gameObject.SetActive(false);
            _sbwGrid.parent.parent.gameObject.SetActive(true);
        }));

        Parent.Find("introduce/Close").GetComponent<Button>().onClick.AddListener((() =>
       {
           _introduce.gameObject.SetActive(false);
       }));

    }

    


    

    private void LoadHeadInfo(Transform grid,List<PersonInfo> infos )
    {
        foreach (PersonInfo info in infos)
        {
            GameObject go = Object.Instantiate(_fixedHeadGo, grid);

            FixedHeadItem item = go.GetComponent<FixedHeadItem>();

            item.SetData(info);

            item.name = info.PersonName;

            var info1 = info;
            item.GetComponent<Button>().onClick.AddListener((() =>
          {
              ShowInfo(info1);
          }));
        }

     
    }

   

    private void ShowInfo(PersonInfo info)
    {
        _introduce.gameObject.SetActive(true);

        _intrHead.sprite = info.headTex;

        _intrName.text = info.PersonName;

        _intrDes.text = info.Describe;
    }

    public override void Enter()
    {
        base.Enter();
        Parent.parent.gameObject.SetActive(true);//父级别也要显示
        ShowImage.gameObject.SetActive(false);
        _introduce.gameObject.SetActive(false);
    }

    public override void Exit()
    {
        base.Exit();
        ShowImage.gameObject.SetActive(true);
    }
}
