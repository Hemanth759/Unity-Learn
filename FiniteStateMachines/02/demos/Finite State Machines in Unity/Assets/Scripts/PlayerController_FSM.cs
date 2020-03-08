using UnityEngine;

public class PlayerController_FSM : MonoBehaviour
{
    #region Player Variables

    public float jumpForce;
    public Transform head;
    public Transform weapon01;
    public Transform weapon02;

    public Sprite idleSprite;
    public Sprite duckingSprite;
    public Sprite jumpingSprite;
    public Sprite spinningSprite;

    private SpriteRenderer face;
    private Rigidbody rbody;

    #endregion

    private PlayerBaseState _currentState;

    public PlayerBaseState CurrentState {
        get {
            return _currentState;
        }
    }

    public Rigidbody Rbody {
        get {
            return rbody;
        }
    }


    public readonly PlayerIdleState idleState = new PlayerIdleState();
    public readonly PlayerJumpingState jumpingState = new PlayerJumpingState();
    public readonly PlayerDuckingState duckingState = new PlayerDuckingState();
    public readonly PlayerSpinningState spinningState = new PlayerSpinningState();


    private void Awake()
    {
        face = GetComponentInChildren<SpriteRenderer>();
        rbody = GetComponent<Rigidbody>();
        SetExpression(idleSprite);
    }

    private void Start()
    {
        TransitionToState(idleState);   
    }

    // Update is called once per frame
    private void Update()
    {
        CurrentState.Update(this);
    }

    private void OnCollisionEnter(Collision other)
    {
        _currentState.onCollisionEnter(this);    
    }

    public void TransitionToState(PlayerBaseState state) {
        _currentState = state;
        _currentState.Enterstate(this);
    }

    public void SetExpression(Sprite newExpression)
    {
        face.sprite = newExpression;
    }
}
