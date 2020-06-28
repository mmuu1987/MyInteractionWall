
using System;
using UnityEngine;
/// <summary>
/// 状态基类,定义了状态的基础方法
/// 为什么需要定义为3个呢
/// 是因为有些逻辑都是进入状态或者退出状态的时候执行的,
/// 所以这里将逻辑分开:
/// 分为:
/// 1>进入状态,
/// 2>状态执行,
/// 3>状态退出
/// </summary>
/// <typeparam name="entity_type">使用泛型,哪个状态使用,就传入哪个状态</typeparam>
public class FsmState<entity_type>
{
   
   
    /// <summary>
    /// 使用状态的对象
    /// </summary>
    public entity_type Target;

    /// <summary>
    /// 存档是否有数据变化
    /// </summary>
    public bool IsChange = false;
    protected FsmState()
    {
       
    } 
    /// <summary>
    /// 进入状态的逻辑
    /// 只执行一次的逻辑
    /// </summary>
    /// <param name="entityType"></param>
    public virtual void Enter()
    {
        Debug.Log(this.ToString()+"    Enter");

       
    }

    /// <summary>
    /// 执行状态的逻辑
    /// </summary>
    /// <param name="entityType"></param>
    public virtual void Excute()
    {
    
    
    }

    /// <summary>
    /// 退出状态的逻辑
    /// </summary>
    /// <param name="entityType"></param>
    public virtual void Exit()
    {
        Debug.Log(this.ToString() + "    Exit");


    }

    /// <summary>
    /// 保存该状态的一些数据
    /// </summary>
    public virtual void SaveData()
    {

    }
    /// <summary>
    /// 显示保存与否对话框   
    /// </summary>
    /// <param name="callback">操作完成保存与否后的回调</param>
    public virtual void ShowSaveConfirm(Action<string> callback = null)
    {
        
    }
}
