using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DeadEventHandler();

public class Player : Character
{
  public event DeadEventHandler Dead;

  public static Player instance;

  public static Player Instance {
    get {
      if (instance == null) {
        instance = GameObject.FindObjectOfType<Player>();
      }

      return instance;
    }
  }

  [SerializeField] private Transform[] groundPoints;

  [SerializeField] private float groundRadius;

  [SerializeField] private LayerMask whatIsGround;

  [SerializeField] public float jumpForce;

  [SerializeField] private bool airControl;

  private float attackTime;

  private SpriteRenderer spriteRenderer;

  private float throwTime;

  private float jumpTime;

  private bool alwaysTrue = true;

  public bool Slide { get; set; }

  public bool Jump { get; set; }

  public bool OnGround { get; set; }

  private Vector3 startPos;
    // Start is called before the first frame update
    public override void Start()
    {
      base.Start();

      startPos = transform.position;

      spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    public void Update() {
      if (!IsDead) {
        HandleInput();

        if (transform.position.y <= -14f) {
          FallingDeath();

          GameObject.FindObjectOfType<Enemy>().Target = null;
        }

      if (Slide && !this.MyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide")) {
        MyAnimator.SetBool("slide", true);
      } else if (!this.MyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide")) {
        MyAnimator.SetBool("slide", false);
        } //this slide function may "break"
      }
    }

    public void FixedUpdate()
    {
      if (!IsDead) {
      OnGround = IsGrounded();

      FindHorizontal();
      
      HandleMovement(horizontal);

      if (!this.MyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !this.MyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Throw")) {
        ChangeDirection(horizontal);
      }

      HandleAttacks();

      ResetValues();
      }
    }

    private void HandleMovement(float horizontal) 
    {
      if (MyRigidbody.velocity.y < 0 && !OnGround) {
        MyAnimator.SetBool("falling", true);

        MyAnimator.SetBool("jump bool", false);
      }

      if (MyRigidbody.velocity.y == 0 && Time.time - jumpTime > 0.10) {
        MyAnimator.SetBool("jump bool", false);
      }

      if (!MyAnimator.GetBool("slide") && (OnGround || airControl)) {
        MyRigidbody.velocity = new Vector2(horizontal * movementSpeed, MyRigidbody.velocity.y);
      }

      if (OnGround && Jump) {
        OnGround = false;

        MyRigidbody.AddForce(new Vector2(0, jumpForce));
      }

      MyAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }

    private void HandleAttacks() {
      if (Attack && !this.MyAnimator.GetCurrentAnimatorStateInfo(0).IsTag("attack")) {
        base.MyAnimator.SetTrigger("attack");

        base.MyAnimator.SetBool("attack bool", true);
      }

      if (Time.time - attackTime > 0.36) {
        base.MyAnimator.SetBool("attack bool", false);
      }

      if (Time.time - throwTime > 0.36) {
        base.MyAnimator.SetBool("throw", false);
      }

      /*if (MyAnimator.GetBool("attack bool") || MyAnimator.GetBool("throw")) {
        if (facingRight) {
          transform.localScale = new Vector2(transform.localScale.x * -1, 1);
        } else if (!facingRight) {
          transform.localScale = new Vector2(transform.localScale.x * 1, 1);
        }
      } */  // doesn't stop player from turning while attacking
    }

    private void HandleInput() {
      if (Input.GetKeyDown(KeyCode.Space)) {
        jumpTime = Time.time;

        Jump = true;

        base.MyAnimator.SetBool("jump bool", true);
      }

      if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time - attackTime > 0.40 && Time.time - throwTime > 0.40) { //this time.time business is potentially flawed
        Attack = true;

        attackTime = Time.time;

        base.MyAnimator.SetBool("attack bool", true);
      }

      if (Input.GetKeyDown(KeyCode.Z)) {
        Slide = true;
      }

      if (Input.GetKeyDown(KeyCode.Tab) && Time.time - throwTime > 0.40 && Time.time - attackTime > 0.40) {
        base.MyAnimator.SetBool("throw", true);

        throwTime = Time.time;
      }
    }

  private bool IsGrounded() {
      foreach (Transform point in groundPoints) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);

        for (int i = 0; i < colliders.Length; i++) {
          if (colliders[i].gameObject != gameObject) {
            /* myAnimator.ResetTrigger("jump"); an example of how to reset triggers*/

            base.MyAnimator.SetBool("falling", false);

            return true;
          }
        }
      }
    
    return false;
  }

  IEnumerator TimerOne() {
    yield return new WaitForSeconds(0.80f);
  }

  public override IEnumerator TakeDamage() {
    if (!MyAnimator.GetBool("slide")) {
      if (canDamage) {
      healthStat.CurrentVal -= 10;

      canDamage = false;

    if (!IsDead) {
      MyAnimator.SetBool("damage", true);

      StartCoroutine(IndicateImmortal());

      StartCoroutine(AnimatorTransitionTimer(0.14f));

      StartCoroutine(TransitionAfterDelay(1.2f));
    } else {
      MyAnimator.SetBool("die", true);

      Death();

      //MyRigidbody.velocity.x = Vector2.zero;

      yield return null;
        }
      }
    }
  }

  public override bool IsDead {
    get {
      return healthStat.CurrentVal <= 0;
    }
  }

  private IEnumerator IndicateImmortal() {
    while (!canDamage) {
      yield return new WaitForSeconds(0.18f);
      spriteRenderer.enabled = false;
      yield return new WaitForSeconds(0.04f);
      spriteRenderer.enabled = true;
    }
  }

  private IEnumerator AnimatorTransitionTimer(float delay) {
    yield return new WaitForSeconds(delay);

    MyAnimator.SetBool("damage", false);
  }

  private void ResetValues() {
    Attack = false;

    base.MyAnimator.ResetTrigger("attack");

    Slide = false;

    Jump = false;
  }

 /*  public void OnDead() {
    if (Dead != null) {
      Dead();
    }
  }*/

  public override void Death() {
    canDamage = true;

    StartCoroutine(Respawn(1.4f));
  }

  public void FallingDeath() {
    canDamage = true;

    StartCoroutine(Respawn(0));
  }

  private IEnumerator Respawn(float delay) {
    yield return new WaitForSeconds(delay);

    healthStat.CurrentVal = healthStat.MaxVal;

    MyAnimator.SetBool("die", false);

    transform.position = startPos;

    MyRigidbody.velocity = Vector2.zero;

    transform.position = transform.position;
  }

  private void OnCollisionEnter2D(Collision2D other) {
    if (other.gameObject.tag == "coin") {
      Destroy(other.gameObject);
    }
  }
}