using System.Collections;
using UnityEngine;

public abstract class State
{
    public StateMachine owner;
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Think() { }
}

public class StateMachine : MonoBehaviour
{
    public State currentState;
    public State globalState;
    public State previousState;

    private IEnumerator _coroutine;

    public int updatesPerSecond = 5;
    void Start() { }

    public void ChangeStateDelayed(State newState, float delay)
    {
        _coroutine = ChangeStateCoRoutine(newState, delay);
        StartCoroutine(_coroutine);
    }

    public void CancelDelayedStateChange()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
    }

    IEnumerator ChangeStateCoRoutine(State newState, float delay)
    {
        yield return new WaitForSeconds(delay);
        ChangeState(newState);
    }

    public void RevertToPreviousState() => ChangeState(previousState);

    public void ChangeState(State newState)
    {
        previousState = currentState;
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.owner = this;
        Debug.Log(currentState.GetType());
        currentState.Enter();
    }

    void Update()
    {
        if (globalState != null)
            globalState.Think();

        if (currentState != null)
            currentState.Think();
    }
}
