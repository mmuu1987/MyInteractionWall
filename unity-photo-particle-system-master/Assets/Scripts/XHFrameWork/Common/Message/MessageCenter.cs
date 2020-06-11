//
// /**************************************************************************
//
// MessageCenter.cs
//
// Author: xiaohong  <704872627@qq.com>
//
// Unity课程讨论群:  152767675
//
// Date: 15-8-16
//
// Description:Provide  functions  to connect Oracle
//
// Copyright (c) 2015 xiaohong
//
// **************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;


namespace XHFrameWork
{
	public class MessageCenter : Singleton<MessageCenter>
	{
		private Dictionary<string, List<MessageEvent>> dicMessageEvents = null;

		public override void Init ()
		{
			dicMessageEvents = new Dictionary<string, List<MessageEvent>>();
		}
		#region Add & Remove Listener

		public void AddListener(string messageName, MessageEvent messageEvent)
		{
			Debug.Log("AddListener Name : " + messageName);
			List<MessageEvent> list = null;
			if (dicMessageEvents.ContainsKey(messageName))
			{
				list = dicMessageEvents[messageName];
			}
			else
			{
				list = new List<MessageEvent>();
				dicMessageEvents.Add(messageName, list);
			}
			// no same messageEvent then add
			if (!list.Contains(messageEvent))
			{
				list.Add(messageEvent);
			}
		}

		public void RemoveListener(string messageName, MessageEvent messageEvent)
		{
			Debug.Log("RemoveListener Name : " + messageName);
			if (dicMessageEvents.ContainsKey(messageName))
			{
				List<MessageEvent> list = dicMessageEvents[messageName];
				if (list.Contains(messageEvent))
				{
					list.Remove(messageEvent);
				}
				if (list.Count <= 0)
				{
					dicMessageEvents.Remove(messageName);
				}
			}
		}

		public void RemoveAllListener()
		{
			dicMessageEvents.Clear();
		}

		#endregion

		#region Send Message

		public void SendMessage(Message message)
		{
			DoMessageDispatcher(message);
		}

		public void SendMessage(string name, object sender)
		{
			SendMessage(new Message(name, sender));
		}

		public void SendMessage(string name, object sender, object content)
		{
			SendMessage(new Message(name, sender, content));
		}

		public void SendMessage(string name, object sender, object content, params object[] dicParams)
		{
			SendMessage(new Message(name, sender, content, dicParams));
		}

		private void DoMessageDispatcher(Message message)
		{
			//Debug.Log("DoMessageDispatcher Name : " + message.Name);
			if (dicMessageEvents == null || !dicMessageEvents.ContainsKey(message.Name))
				return;
			List<MessageEvent> list = dicMessageEvents[message.Name];
			for (int i=0; i<list.Count; i++)
			{
				MessageEvent messageEvent = list[i];
				if (null != messageEvent)
				{
					messageEvent(message);
				}
			}
		}

		#endregion

	}
}

