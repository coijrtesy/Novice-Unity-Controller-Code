using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeState : IEnemyState
{
  private float attackTimer;

  private float attackCooldown = 3;

  private float animaionTimer = 0.36f;

  private bool canAttack = true;

  private Enemy enemy;

  public void Execute() {
    Attack();

    if (enemy.InThrowRange && !enemy.InMeleeRange) {
      enemy.ChangeState(new RangedSate()); 
    } 
    
    if (enemy.Target == null) {
      enemy.ChangeState(new IdleSate());
    }
  }
  public void Enter(Enemy enemy) {
    this.enemy = enemy;

    enemy.MyAnimator.SetBool("throw", false);
  }
  public void Exit() {

  }
  public void OnTriggerEnter(Collider2D other) {
    
  }

  private void Attack() {
    attackTimer += Time.deltaTime;

    if (attackTimer >= attackCooldown) {
      canAttack = true;
    }

    if (attackTimer >= animaionTimer) {
      enemy.MyAnimator.SetBool("attack", false);
    }

    if (canAttack && enemy.Target != null && enemy.InMeleeRange) {
      canAttack = false;

      enemy.MyAnimator.SetBool("attack", true);

      attackTimer = 0;
    }
  }
}

