using System;
using UnityEngine;

public abstract class BaseState<Estate> where Estate : Enum {

    private UpdateMode mode;

    public enum UpdateMode {
        Update,
        FixedUpdate,
        LateUpdate
    }

    public Estate stateKey { get; private set; }

    public BaseState(Estate key) : this(key, UpdateMode.Update) { }

    public BaseState(Estate key, UpdateMode mode) {
        stateKey = key;
        this.mode = mode;
    }

    public bool OnUpdateMode() {
        return mode.Equals(UpdateMode.Update);
    }

    public bool OnFixedUpdateMode()
    {
        return mode.Equals(UpdateMode.FixedUpdate);
    }

    public bool OnLateUpdateMode()
    {
        return mode.Equals(UpdateMode.LateUpdate);
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public abstract void UpdateState();
    public abstract Estate GetNextState();
    public virtual void OnTriggerEnter(Collider other) { }
    public virtual void OnTriggerStay(Collider other) { }
    public virtual void OnTriggerExit(Collider other) { }
}
