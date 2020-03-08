using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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

    #endregion

    private SpriteRenderer face;
    private Rigidbody rbody;
    private float rotation;
    private bool isJumping;

    private void Awake()
    {
        face = GetComponentInChildren<SpriteRenderer>();
        rbody = GetComponent<Rigidbody>();
        SetExpression(idleSprite);
    }

    void Start()
    {
        isJumping = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump")) {
            if (isJumping) 
                return;
            isJumping = true;
            SetExpression(jumpingSprite);
            rbody.AddForce(Vector3.up * jumpForce);
        }

        if (Input.GetButtonDown("Duck")) {
            if (!isJumping) {
                SetExpression(duckingSprite);
                head.localPosition = new Vector3(head.localPosition.x, .5f, head.localPosition.z);
            }
        }
        if (Input.GetButtonUp("Duck")) {
            SetExpression(idleSprite);
            head.localPosition = new Vector3(head.localPosition.x, .8f, head.localPosition.z);
        }
    }

    private void spin() {
        
    }

    void OnCollisionEnter(Collision other)
    {
        isJumping = false;
        SetExpression(idleSprite);
    }

    public void SetExpression(Sprite newExpression)
    {
        face.sprite = newExpression;
    }
}
