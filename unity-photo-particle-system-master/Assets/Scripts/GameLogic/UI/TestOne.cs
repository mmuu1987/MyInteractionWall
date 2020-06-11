using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XHFrameWork;

public class TestOne : BaseUI 
{
	private TestOneModule oneModule;

	private Button btn;
	private Text text;
	#region implemented abstract members of BaseUI
	public override EnumUIType GetUIType ()
	{
		return EnumUIType.TestOne;
	}
	#endregion

	// Use this for initialization
	void Start ()
	{
//		btn = transform.Find("Panel/Button").GetComponent<Button>();
//		btn.onClick.AddListener(OnClickBtn);

		text = transform.Find("Panel/Text").GetComponent<Text>();

		//EventTriggerListener.Get(transform.Find("Panel/Button").gameObject).SetEventHandle(EnumTouchEventType.OnClick, Close);
	
		EventTriggerListener listener = EventTriggerListener.Get(transform.Find("Panel/Button").gameObject);
		listener.SetEventHandle(EnumTouchEventType.OnClick, Close, 1, "1234");

		oneModule = ModuleManager.Instance.Get<TestOneModule>();
		text.text = "Gold: " + oneModule.Gold;

	}

	protected override void OnAwake ()
	{
		MessageCenter.Instance.AddListener("AutoUpdateGold", UpdateGold);
		base.OnAwake ();
	}

	protected override void OnRelease ()
	{
		MessageCenter.Instance.RemoveListener("AutoUpdateGold", UpdateGold);
		base.OnRelease ();
	}

	private void UpdateGold(Message message)
	{
		int gold = (int) message["gold"];
		Debug.Log("TestOne UpdateGold : " + gold);
		text.text = "Gold: " + gold;
	}

	private void OnClickBtn()
	{
		UIManager.Instance.OpenUICloseOthers(EnumUIType.TestTwo);
//		GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/TestUITwo"));
//		TestTwo to = go.GetComponent<TestTwo>();
//		if (null == to)
//			to = go.AddComponent<TestTwo>();
//		Close();
	}

//	private void Close()
//	{
//		Destroy(gameObject);
//	}
	private void Close(GameObject _listener, object _args, params object[] _params)
	{
		int i = (int) _params[0];
		string s = (string) _params[1];
		Debug.Log(i);
		Debug.Log(s);
		UIManager.Instance.OpenUICloseOthers(EnumUIType.TestTwo);
	}
}

