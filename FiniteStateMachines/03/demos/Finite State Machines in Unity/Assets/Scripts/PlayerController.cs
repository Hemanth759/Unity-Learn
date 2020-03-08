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

    private SpriteRenderer face;
    private Rigidbody rbody;

    #endregion

    private float rotation;

    private bool isJumping;
    private bool isDucking;
    private bool isSpinning;

    private void Awake()
    {
        face = GetComponentInChildren<SpriteRenderer>();
        rbody = GetComponent<Rigidbody>();
        SetExpression(idleSprite);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isJumping == false && isDucking == false)
            {
                isJumping = true;
                SetExpression(jumpingSprite);
                rbody.AddForce(Vector3.up * jumpForce);
            }
        }
        else if (Input.GetButtonDown("Duck"))
        {
            if (isJumping == false)
            {
                isDucking = true;
                head.localPosition = new Vector3(head.localPosition.x, .5f, head.localPosition.z);
                SetExpression(duckingSprite);
            }
            else
            {
                isSpinning = true;
                SetExpression(spinningSprite);
            }
        }
        else if (Input.GetButtonUp("Duck"))
        {
            if (isJumping == false)
            {
                isDucking = false;
                head.localPosition = new Vector3(head.localPosition.x, .8f, head.localPosition.z);
                SetExpression(idleSprite);
            }
        }
        else if(Input.GetButtonDown("SwapWeapon"))
        {
            if (isJumping == false && isDucking == false && isSpinning == false)
            {
                bool usingWeapon01 = weapon01.gameObject.activeInHierarchy;

                weapon01.gameObject.SetActive(usingWeapon01 == false);
                weapon02.gameObject.SetActive(usingWeapon01);
            }
        }

        if(isSpinning == true)
        {
            Spin();
        }
    }

    private void Spin()
    {
        float amountToRotate = 900 * Time.deltaTime;
        rotation += amountToRotate;

        if (rotation < 360)
        {
            transform.Rotate(Vector3.up, amountToRotate);
        }
        else
        {
            transform.rotation = Quaternion.identity;
            isSpinning = false;
            rotation = 0;
            SetExpression(jumpingSprite);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isJumping = false;
        SetExpression(idleSprite);
    }

    public void SetExpression(Sprite newExpression)
    {
        face.sprite = newExpression;
    }
}
