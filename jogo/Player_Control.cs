using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class Player_Control : MonoBehaviour
{
    [Header("Physics variables")]
    [SerializeField]private float walkSpeed = 10;
    [SerializeField]private float jumpForce = 45;
    private int JumpBufferCounter;
    [SerializeField] private int JumpBufferFrames;
    private float cayoteTimeCounter;
    [SerializeField] private float cayoteTime = 0;
    private int AirJumpCounter = 1;
    [SerializeField] private int maxAirJumping;

    [Header("Ground check field")]
    [SerializeField] private Transform Ground_check;
    [SerializeField] private float GroundY = 0.2f; 
    [SerializeField] private float GroundX = 0.5f;
    [SerializeField] private LayerMask WhatIsGround;

    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCoolDown;

    PlayerStation pState;
    private Rigidbody2D rb;
    private float Axis;
    Animator anim;
    private bool canDash;
    private float gravity;
    private bool dashed;
    
    public static Player_Control Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        pState = GetComponent<PlayerStation>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;
    }   

    void Update()
    {
        Get_input();
        UpdateVariables();
        if (pState.dashing) return;
        Move();
        Jump();
        Flip();
        startDash();
    }
    
     private void Get_input()
    {
        Axis = Input.GetAxis("Horizontal");
    }

    void Flip()
    {
        if (Axis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
    }
    void Move()
    {
        rb.velocity = new Vector2(walkSpeed * Axis, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    void startDash()
    {
        if(Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded())
        {
            dashed = false;
        }
    }
    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }


    public bool Grounded()
    {
        if (Physics2D.Raycast(Ground_check.position, Vector2.down, GroundY, WhatIsGround) 
            || Physics2D.Raycast(Ground_check.position + new Vector3(GroundX,0,0), Vector2.down, GroundY, WhatIsGround)
            || Physics2D.Raycast(Ground_check.position + new Vector3(-GroundX, 0, 0), Vector2.down, GroundY, WhatIsGround))
        {
            return true;
        } 
        else
        {
            return false;
        }
    }

    void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            pState.jump = false;
        }
        
        if (!pState.jump)
        {
            if (JumpBufferCounter > 0 && cayoteTimeCounter > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
                pState.jump = true;
            }
            else if(Grounded() && AirJumpCounter < maxAirJumping && Input.GetButtonDown("Jump"))
            {
                pState.jump = true;
                AirJumpCounter++;
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            }
        }

        anim.SetBool("Jumping", !Grounded());
    }

    void UpdateVariables()
    {
        if (Grounded())
        {
            pState.jump = false;
            cayoteTimeCounter = cayoteTime;
            AirJumpCounter = 0;
        }
        else
        {
            cayoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            JumpBufferCounter = JumpBufferFrames;
        }
        else
        {
            JumpBufferCounter--;
        }
    }


}
