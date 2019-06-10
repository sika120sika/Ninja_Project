using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State<T>
{
    //このステートを利用するインスタンス
    protected T owner;

    public State(T owner)
    {
        this.owner = owner;
    }
    //このステートに遷移するときに一度だけ呼ばれる
    public virtual void Enter() { }
    //このステートである間マイフレーム呼ばれる
    public virtual void Execute() { }
    //このステートから他のステートに遷移するときに一度だけ呼ばれる
    public virtual void Exit() { }
}

public class StateMachine <T>
{
    private State<T> currentState;
    
    public StateMachine()
    {
        currentState = null;
    }
    public State<T> CurrentState
    {
        get { return currentState; }
    }
    //ステートを遷移
    public void ChangeState(State<T> state)
    {
        if(currentState != null)
        {
            currentState.Exit();
        }
        currentState = state;
        currentState.Enter();
    }
    public void Update()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }
    }
}
public class StatefulObjectBase <T,Tenum> : MonoBehaviour where T:class where Tenum : System.IConvertible
{
    protected List<State<T>> stateList = new List<State<T>>();
    protected StateMachine<T> stateMachine;
    public virtual void ChangeState(Tenum state)
    {
        if(stateMachine == null)
        {
            return;
        }
        stateMachine.ChangeState(stateList[state.ToInt32(null)]);
    }
    public virtual bool IsCurretState(Tenum state)
    {
        if(stateMachine == null)
        {
            return false;
        }
        return stateMachine.CurrentState == stateList[state.ToInt32(null)];
    }
    protected virtual void Update()
    {
        if(stateMachine != null)
        {
            stateMachine.Update();
        }
    }
}