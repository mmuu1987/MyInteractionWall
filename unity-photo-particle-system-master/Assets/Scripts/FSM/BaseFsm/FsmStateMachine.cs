using UnityEngine;
using System.Collections;

/// <summary>
/// 状态机
/// 使用这个来驱动各种状态
/// </summary>
/// <typeparam name="entity_type"></typeparam>
public class FsmStateMachine<entity_type>
{
    //状态持有对象
    private entity_type m_pOwner;

    //
    private FsmState<entity_type> m_pCurrentState;//当前状态
    private FsmState<entity_type> m_pPreviouState;//上一个状态
    private FsmState<entity_type> m_pGlobalState;//全局状态


    /// <summary>
    /// 状态机的构造函数
    /// </summary>
    /// <param name="owner"></param>
    public FsmStateMachine(entity_type owner)
    {
        m_pOwner = owner;
        m_pPreviouState = null;
        m_pCurrentState = null;
        m_pGlobalState = null;
    }

    /// <summary>
    /// 设置当前状态
    /// </summary>
    /// <param name="GlobalState"></param>
    public void SetCurrentState(FsmState<entity_type> CurrentState)
    {
        if (CurrentState != null)
        {
            //保存 当前状态
            m_pCurrentState = CurrentState;
            //设置 状态中的 Target 为
            m_pCurrentState.Target = m_pOwner;
            m_pCurrentState.Enter();
        }
    }


    /// <summary>
    /// 进入全局状态
    /// 进行一些数据的刷新,比如说距离的计算等
    /// </summary>
    /// <param name="GlobalState"></param>
    public void SetGlobalState(FsmState<entity_type> GlobalState)
    {
        if (GlobalState != null)
        {
            m_pGlobalState = GlobalState;
            m_pGlobalState.Target = m_pOwner;
           // m_pGlobalState.Enter();
        }
        else
        {
            Debug.LogError("不能设置空状态");
        }
    }


    /// <summary>
    /// 进入全局状态d
    /// </summary>
    public void GlobalStateEnter()
    {
        m_pGlobalState.Enter();
    }
    /// <summary>
    /// 退出全局状态
    /// </summary>
    public void GlobalStateExit()
    {
        m_pGlobalState.Exit();
    }





    /// <summary>
    /// Update方法
    /// </summary>
    public void SmUpdate()
    {
        //只要设置了.就会一直执行
        if (m_pGlobalState != null)
        {
            m_pGlobalState.Excute();
        }
        if (m_pCurrentState != null)
        {
            m_pCurrentState.Excute();
        }
    }


    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="pNewState">希望变成的状态</param>
    public void ChangeState(FsmState<entity_type> pNewState)
    {

        if (pNewState == null)
        {
            Debug.Log("不能使用空的状态");
        }
        //触发状态退出方法
        m_pCurrentState.Exit();

        //保存上一状态
        m_pPreviouState = m_pCurrentState;

        //设置新状态为当前状态
        m_pCurrentState = pNewState;
        //传递 状态 使用 对象
        m_pCurrentState.Target = m_pOwner;

        //触发当前状态调用Enter方法
        m_pCurrentState.Enter();
    }

    /// <summary>
    /// 切换回上一状态
    /// </summary>
    public void RevertToPreviousState()
    {
        this.ChangeState(m_pPreviouState);
    }

    /// <summary>
    /// 获取当前状态
    /// </summary>
    /// <returns></returns>
    public FsmState<entity_type> Get_CurrentState()
    {
        return m_pCurrentState;
    }

    public FsmState<entity_type> Get_GlobalState()
    {
        return m_pGlobalState;
    }



    /// <summary>
    /// 获取上一状态
    /// </summary>
    /// <returns></returns>
    public FsmState<entity_type> Get_PreviousState()
    {
        return m_pPreviouState;
    }
}
