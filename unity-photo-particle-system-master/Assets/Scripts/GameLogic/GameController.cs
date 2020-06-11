using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XHFrameWork;
using System;

public class GameController : DDOLSingleton<GameController> {

	// Use this for initialization
	void Start () {
		
		ModuleManager.Instance.RegisterAllModules();

		SceneManager.Instance.RegisterAllScene();



//		UIManager.Instance.OpenUI(EnumUIType.TestOne);
//		GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/TestUIOne"));
//		TestOne to = go.GetComponent<TestOne>();
//		if (null == to)
//			to = go.AddComponent<TestOne>();

		//GameController.Instance.StartCoroutine(AsyncLoadData());

		//DDOLTest.Instance.StartCoroutine_Auto(AsyncLoadData());

//		float time = System.Environment.TickCount;
//		for(int i=1; i<1000; i++)
//		{
//			GameObject go = null;
////			// 1 60000
////			go = Instantiate(Resources.Load<GameObject>("Prefabs/Cube"));
////			go.transform.position = UnityEngine.Random.insideUnitSphere * 20;
//
////			// 2 60000
////			go = ResManager.Instance.LoadInstance("Prefabs/Cube") as GameObject;
////			go.transform.position = UnityEngine.Random.insideUnitSphere * 20;
//
////			// 3 35000
////			ResManager.Instance.LoadAsyncInstance("Prefabs/Cube", (_obj)=>{
////				go = _obj as GameObject;
////				go.transform.position = UnityEngine.Random.insideUnitSphere * 20;
////			});
//
////			// 4 20000
////			ResManager.Instance.LoadCoroutineInstance("Prefabs/Cube", (_obj)=>{
////				go = _obj as GameObject;
////				go.transform.position = UnityEngine.Random.insideUnitSphere * 20;
////			});
//		}
//
//		Debug.Log("Times： " + (System.Environment.TickCount - time) * 1000);
//
//		StartCoroutine(AutoUpdateGold());
	}







	private IEnumerator AutoUpdateGold()
	{
		int gold = 0;
		while(true)
		{
			gold ++;
			yield return new WaitForSeconds(1.0f);
			Message message = new Message(MessageType.Net_MessageTestOne.ToString(), this);
			message["gold"] = gold;
			MessageCenter.Instance.SendMessage(message);
//			Message message = new Message(MessageType.Net_MessageTestOne, this);
//			message["gold"] = gold;
//			message.Send();
		}
	}

	private IEnumerator<int> AsyncLoadData()
	{
		int i = 0;
		while(true)
		{
			Debug.Log("------> " + i);
			yield return i;
			i++;
		}
	}

}
