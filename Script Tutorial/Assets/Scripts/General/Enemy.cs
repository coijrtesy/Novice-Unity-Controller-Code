using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
  [SerializeField] private float meleeRange;

  [SerializeField] private float throwRange;

  [SerializeField] private Transform leftEdge;

  [SerializeField] private Transform rightEdge;
  
  private Canvas enemyCanvas;

  public bool InMeleeRange {
    get {
      if (Target != null) {
        return Vector2.Distance(transform.position, Target.transform.position) <= meleeRange;
      } else {
        return false;
      }
    }
  }

  public bool InThrowRange {
    get {
      if (Target != null) {
        return Vector2.Distance(transform.position, Target.transform.position) <= throwRange;
      } else {
        return false;
      }
    }
  }

  public GameObject Target { get; set; }

  private IEnemyState currentState;

  public override void Start()
    {
        base.Start();

        ChangeState(new IdleSate());

        Player.Instance.Dead += new DeadEventHandler(RemoveTarget);

        enemyCanvas = transform.GetComponentInChildren<Canvas>();

        enemyCanvas.enabled = false;
    }

    void Update()
    {
      if (!IsDead) {
        currentState.Execute();

        LookAtTarget();
      }
    }

    private void LookAtTarget () {
      if (Target != null) {
        float xDir = Target.transform.position.x - transform.position.x;

        if (xDir < 0 && facingRight || xDir > 0 && !facingRight) {
          ChangeDirectionTwo();
        }
      }
    }

    public void RemoveTarget () {
      Target = null;

      ChangeState(new IdleSate());
    }

    public void ChangeState(IEnemyState newState) {
      if (currentState != null) {
        currentState.Exit();
      }

      currentState = newState;

      currentState.Enter(this);
    }

  public void Move() {
    if (!Attack) {
      MyAnimator.SetFloat("speed 1", 1);

      if ((GetDirection().x > 0 && transform.position.x < rightEdge.position.x) || (GetDirection().x < 0 && transform.position.x > leftEdge.position.x)) {
        transform.Translate(GetDirection() * (movementSpeed * Time.deltaTime));
      } else if (currentState is PatrolState) {
        ChangeDirectionTwo();
      }
    }
  }

  public Vector2 GetDirection() {
    return facingRight ? Vector2.right : Vector2.left;
  }

  public override void OnTriggerEnter2D(Collider2D other) {
    base.OnTriggerEnter2D(other);

    currentState.OnTriggerEnter(other);
  }

  public void ChangeDirectionTwo() {
    facingRight = !facingRight; 

    transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
    
    enemyCanvas.transform.localScale = new Vector3(enemyCanvas.transform.localScale.x * -1, 0.0055f, 1);
  }

  public override IEnumerator TakeDamage() {
    if (!enemyCanvas.isActiveAndEnabled) {
      enemyCanvas.enabled = true;
    }

    if (canDamage) {
      healthStat.CurrentVal -= 10;

      canDamage = false;

    if (!IsDead) {
      MyAnimator.SetBool("damage", true);
      StartCoroutine(TransitionAfterDelay(0.12f));
    } else {
      MyAnimator.SetBool("die", true);
      
      enemyCanvas.enabled = false;

      yield return null;
      }
    }
  }

  protected override IEnumerator TransitionAfterDelay(float delay) {
    yield return new WaitForSeconds(delay);

    MyAnimator.SetBool("damage", false);

    canDamage = true;
  }

  public override bool IsDead {
    get {
      return healthStat.CurrentVal <= 0;
    }
  }

  public override void Death() {
    Destroy(gameObject);
  }
}
