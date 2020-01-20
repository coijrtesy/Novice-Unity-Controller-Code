using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
  [SerializeField] protected Stat healthStat;

  [SerializeField] protected GameObject meleePrefab;

  [SerializeField] protected Transform meleeAltSpawnPos;

  public bool TakingDamage { get; set; }

  public abstract bool IsDead { get; }

  public Animator MyAnimator { get; private set; }

  public Rigidbody2D MyRigidbody { get; set; }

  [SerializeField] public float movementSpeed;

  protected bool facingRight;

  protected bool canDamage;

  [SerializeField] public List<string> damageSources;

  public float horizontal { get; set; }

  public bool Attack { get; set; }

  [SerializeField] protected GameObject projectilePrefab;

  public Vector3 tmpPos;

    public virtual void Start()
    {
      facingRight = true;
        
      MyAnimator = GetComponent<Animator>();

      MyRigidbody = GetComponent<Rigidbody2D>();

      canDamage = true;

      healthStat.Initialize();
    }

    public virtual void FixedUpdate()
    {
    }

    public void ChangeDirection(float horizontal) {
      if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) {
        facingRight = !facingRight;

      transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
      }
    }

    public void FindHorizontal() {
      horizontal = Input.GetAxis("Horizontal");
    }

    public void CastProjectile() {
    if(facingRight) {
      GameObject tmp = (GameObject)Instantiate(projectilePrefab, transform.position, Quaternion.identity);
      tmp.GetComponent<Projectile>().Initialize(Vector2.right);
    } else {
      GameObject tmp = (GameObject)Instantiate(projectilePrefab, transform.position, Quaternion.identity);
      tmp.GetComponent<Projectile>().Initialize(Vector2.left);
      }
    }

    public void CastMelee() {
      if(facingRight) {
      GameObject tmp = (GameObject)Instantiate(meleePrefab, meleeAltSpawnPos.position, Quaternion.identity);
      tmp.GetComponent<MeleeAlternative>().Initialize(Vector2.right);
    } else {
      GameObject tmp = (GameObject)Instantiate(meleePrefab, meleeAltSpawnPos.position, Quaternion.identity);
      tmp.GetComponent<MeleeAlternative>().Initialize(Vector2.left);
      }
    }

  public virtual IEnumerator TakeDamage() {
    if (canDamage) {
      healthStat.CurrentVal -= 10;

      canDamage = false;

    if (!IsDead) {
      MyAnimator.SetBool("damage", true);
      StartCoroutine(TransitionAfterDelay(0.12f));
    } else {
      MyAnimator.SetBool("die", true);
      yield return null;
      }
    }
  }

  public virtual void OnTriggerEnter2D (Collider2D other) {
    if (damageSources.Contains(other.tag)) {
      StartCoroutine(TakeDamage());
    }
  }

  

  protected virtual IEnumerator TransitionAfterDelay(float delay) {
    yield return new WaitForSeconds(delay);

    MyAnimator.SetBool("damage", false);

    canDamage = true;
  }

  public abstract void Death();

 /*  public void MeleeAttack() {
    swordCollider.enabled = !swordCollider.enabled;

    //tmpPos = swordCollider.transform.position;
  } */

 /*  public void DamageTarget() {
    swordCollider.transform.position = new Vector3(swordCollider.transform.position.x + 0.01f, swordCollider.transform.position.y, swordCollider.transform.position.z);
  } */

  /* public void DisableMeleeAttack() {
    swordCollider.enabled = false;

    swordCollider.transform.position = tmpPos;
  } */
}
