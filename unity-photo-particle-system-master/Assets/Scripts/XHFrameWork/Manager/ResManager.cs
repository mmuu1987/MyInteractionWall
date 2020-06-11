//
// /**************************************************************************
//
// ResManager.cs
//
// Author: xiaohong  <704872627@qq.com>
//
// Unity课程讨论群:  152767675
//
// Date: 15-8-6
//
// Description:Provide  functions  to connect Oracle
//
// Copyright (c) 2015 xiaohong
//
// **************************************************************************/


using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace XHFrameWork
{
	public class AssetInfo
	{
		private UnityEngine.Object _Object;
		public Type AssetType { get; set; }
		public string Path { get; set; }
		public int RefCount { get; set; }
		public bool IsLoaded 
		{
			get 
			{
				return null != _Object;
			}
		}

		public UnityEngine.Object AssetObject
		{
			get
			{
				if (null == _Object)
				{
					_ResourcesLoad();
				}
				return _Object;
			}
		}

		public IEnumerator GetCoroutineObject(Action<UnityEngine.Object> _loaded)
		{
			while (true)
			{
				yield return null;
				if (null == _Object)
				{
					//yield return null;
					_ResourcesLoad();
					yield return null;
				}
				if (null != _loaded)
					_loaded(_Object);
				yield break;
			}

		}

		private void _ResourcesLoad()
		{
			try {
				_Object = Resources.Load(Path);
				if (null == _Object)
					Debug.Log("Resources Load Failure! Path:" + Path);
			}
			catch(Exception e)
			{
				Debug.LogError(e.ToString());
			}
		}

		public IEnumerator GetAsyncObject(Action<UnityEngine.Object> _loaded)
		{
			return GetAsyncObject(_loaded, null);
		}

		public IEnumerator GetAsyncObject(Action<UnityEngine.Object> _loaded, Action<float> _progress)
		{
			// have Object
			if (null != _Object)
			{
				_loaded(_Object);
				yield break;
			}

			// Object null. Not Load Resources
		    ResourceRequest _resRequest = Resources.LoadAsync(Path);

			// 
			while (_resRequest.progress < 0.9)
			{
				if (null != _progress)
					_progress(_resRequest.progress);
				yield return null;
			}

			// 
			while (!_resRequest.isDone)
			{
				if (null != _progress)
					_progress(_resRequest.progress);
				yield return null;
			}

			// ???
			_Object = _resRequest.asset;
			if (null != _loaded)
				_loaded(_Object);

			yield return _resRequest;
		}
	}

	public class ResManager : Singleton<ResManager>
	{
		private Dictionary<string, AssetInfo> dicAssetInfo = null;

		public override void Init ()
		{
			dicAssetInfo = new Dictionary<string, AssetInfo>();
		}

		#region Load Resources & Instantiate Object

		/// <summary>
		/// Loads the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		/// <param name="_path">_path.</param>
		public UnityEngine.Object LoadInstance(string _path)
		{
			UnityEngine.Object _obj = Load(_path);
			return Instantiate(_obj);
		}

		/// <summary>
		/// Loads the coroutine instance.
		/// </summary>
		/// <param name="_path">_path.</param>
		/// <param name="_loaded">_loaded.</param>
		public void LoadCoroutineInstance(string _path, Action<UnityEngine.Object> _loaded)
		{
			LoadCoroutine(_path, (_obj)=>{ Instantiate(_obj, _loaded); });
		}

		/// <summary>
		/// Loads the async instance.
		/// </summary>
		/// <param name="_path">_path.</param>
		/// <param name="_loaded">_loaded.</param>
		public void LoadAsyncInstance(string _path, Action<UnityEngine.Object> _loaded)
		{
			LoadAsync(_path, (_obj)=>{ Instantiate(_obj, _loaded); });
		}

		/// <summary>
		/// Loads the async instance.
		/// </summary>
		/// <param name="_path">_path.</param>
		/// <param name="_loaded">_loaded.</param>
		/// <param name="_progress">_progress.</param>
		public void LoadAsyncInstance(string _path, Action<UnityEngine.Object> _loaded, Action<float> _progress)
		{
			LoadAsync(_path, (_obj)=>{ Instantiate(_obj, _loaded); } , _progress);
		}
		#endregion
	
		#region Load Resources
		/// <summary>
		/// Load the specified _path.
		/// </summary>
		/// <param name="_path">_path.</param>
		public UnityEngine.Object Load(string _path)
		{
			AssetInfo _assetInfo = GetAssetInfo(_path);
			if (null != _assetInfo)
				return _assetInfo.AssetObject;
			return null;
		}
		#endregion

		#region Load Coroutine Resources

		/// <summary>
		/// Loads the coroutine.
		/// </summary>
		/// <param name="_path">_path.</param>
		/// <param name="_loaded">_loaded.</param>
		public void LoadCoroutine(string _path, Action<UnityEngine.Object> _loaded)
		{
			AssetInfo _assetInfo = GetAssetInfo(_path, _loaded);
			if (null != _assetInfo)
				CoroutineController.Instance.StartCoroutine(_assetInfo.GetCoroutineObject(_loaded));
		}
		#endregion

		#region Load Async Resources

		/// <summary>
		/// Loads the async.
		/// </summary>
		/// <param name="_path">_path.</param>
		/// <param name="_loaded">_loaded.</param>
		public void LoadAsync(string _path, Action<UnityEngine.Object> _loaded)
		{
			LoadAsync(_path, _loaded, null);
		}

		/// <summary>
		/// Loads the async.
		/// </summary>
		/// <param name="_path">_path.</param>
		/// <param name="_loaded">_loaded.</param>
		/// <param name="_progress">_progress.</param>
		public void LoadAsync(string _path, Action<UnityEngine.Object> _loaded, Action<float> _progress)
		{
			AssetInfo _assetInfo = GetAssetInfo(_path, _loaded);
			if (null != _assetInfo)
				CoroutineController.Instance.StartCoroutine(_assetInfo.GetAsyncObject(_loaded, _progress));
		}
		#endregion

		#region Get AssetInfo & Instantiate Object

		/// <summary>
		/// Gets the asset info.
		/// </summary>
		/// <returns>The asset info.</returns>
		/// <param name="_path">_path.</param>
		private AssetInfo GetAssetInfo(string _path)
		{
			return GetAssetInfo(_path, null);
		}

		/// <summary>
		/// Gets the asset info.
		/// </summary>
		/// <returns>The asset info.</returns>
		/// <param name="_path">_path.</param>
		/// <param name="_loaded">_loaded.</param>
		private AssetInfo GetAssetInfo(string _path, Action<UnityEngine.Object> _loaded)
		{
			if (string.IsNullOrEmpty(_path))
			{
				Debug.LogError("Error: null _path name.");
				if (null != _loaded)
					_loaded(null);
			}
			// Load Res....
			AssetInfo _assetInfo = null;
			if (!dicAssetInfo.TryGetValue(_path, out _assetInfo))
			{
				_assetInfo = new AssetInfo();
				_assetInfo.Path = _path;
				dicAssetInfo.Add(_path, _assetInfo);
			}
			_assetInfo.RefCount++;
			return _assetInfo;
		}

		/// <summary>
		/// Instantiate the specified _obj.
		/// </summary>
		/// <param name="_obj">_obj.</param>
		private UnityEngine.Object Instantiate(UnityEngine.Object _obj)
		{
			return Instantiate(_obj, null);
		}

		/// <summary>
		/// Instantiate the specified _obj and _loaded.
		/// </summary>
		/// <param name="_obj">_obj.</param>
		/// <param name="_loaded">_loaded.</param>
		private UnityEngine.Object Instantiate(UnityEngine.Object _obj, Action<UnityEngine.Object> _loaded)
		{
			UnityEngine.Object _retObj = null;
			if (null != _obj)
			{
				_retObj = MonoBehaviour.Instantiate(_obj);
				if (null != _retObj)
				{
					if (null != _loaded)
					{
						_loaded(_retObj);
						return null;
					}
					return _retObj;
				}
				else
				{
					Debug.LogError("Error: null Instantiate _retObj.");
				}
			}
			else
			{
				Debug.LogError("Error: null Resources Load return _obj.");
			}
			return null;
		}

		#endregion

	}
}

