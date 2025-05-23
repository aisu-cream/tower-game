using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerStateMachine : FiniteStateMachine<PlayerStateMachine.PlayerState>
{
    [SerializeField] WalkState walkState = new WalkState(PlayerState.Walk);
    [SerializeField] RunState runState = new RunState(PlayerState.Run);

    private static Rigidbody2D rb;

    public enum PlayerState
    {
        Idle,
        Walk,
        Run
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        AddState(PlayerState.Idle, new IdleState(PlayerState.Idle));
        AddState(PlayerState.Walk, walkState);
        AddState(PlayerState.Run, runState);

        SetCurrentState(PlayerState.Idle);

        base.Start();
    }

    private class IdleState : BaseState<PlayerState>
    {
        public IdleState(PlayerState stateKey) : base(stateKey) { }

        public override void EnterState() {
            rb.linearVelocity = Vector3.zero;
        }

        public override void UpdateState() { }

        public override PlayerState GetNextState() {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
                if (Input.GetKey(KeyCode.LeftShift))
                    return PlayerState.Run;
                else
                    return PlayerState.Walk;
            }
            else
                return stateKey;
        }
    }

    [Serializable]
    private class WalkState : BaseState<PlayerState>
    {
        public float maxSpeed = 5;
        public float accelRate = 6;
        public float decelRate = 6;

        protected Vector2 moveDir;
        protected Vector2 moveForce;
        
        public WalkState(PlayerState stateKey) : base(stateKey, UpdateMode.FixedUpdate) {
            moveDir = Vector2.zero;
            moveForce = Vector2.zero;
        }
        
        public override void UpdateState()
        {
            // accelerate player to the target velocity
            moveDir.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            moveDir.Normalize();

            float horForce = moveDir.x * maxSpeed - rb.linearVelocity.x;
            float vertForce = moveDir.y * maxSpeed - rb.linearVelocity.y;

            if (moveDir.magnitude > 0)
            {
                horForce *= accelRate;
                vertForce *= accelRate;
            }
            else
            {
                horForce *= decelRate;
                vertForce *= decelRate;
            }

            moveForce.Set(horForce, vertForce);
            rb.AddForce(moveForce);
        }

        public override PlayerState GetNextState() {
            if (moveDir.magnitude == 0 && Mathf.Approximately(0, rb.linearVelocity.magnitude))
                return PlayerState.Idle;
            else if (moveDir.magnitude != 0 && Input.GetKey(KeyCode.LeftShift))
                return PlayerState.Run;
            else
                return stateKey;
        }
    }

    [Serializable]
    private class RunState : WalkState
    {
        public RunState(PlayerState stateKey) : base(stateKey) { }

        public override PlayerState GetNextState() {
            if (moveDir.magnitude == 0 && Mathf.Approximately(0, rb.linearVelocity.magnitude))
                return PlayerState.Idle;
            else if (moveDir.magnitude != 0 && !Input.GetKey(KeyCode.LeftShift))
                return PlayerState.Walk;
            else
                return stateKey;
        }
    }
}
