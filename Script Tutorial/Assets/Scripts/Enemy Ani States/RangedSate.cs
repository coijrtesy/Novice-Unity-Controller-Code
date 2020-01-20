using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedSate : IEnemyState
{
private float throwTimer;

private float throwCooldown = 3;

private float animationTimer = 0.36f;

private bool canThrow = true;

  private Enemy enemy;
  public void Execute() {
    CastProjectile();

    if (enemy.InMeleeRange) {
      enemy.ChangeState(new MeleeState());
    }

    if (enemy.Target != null) {
      enemy.Move();
    } 
    
    if (enemy.Target == null) {
      enemy.ChangeState(new IdleSate());
    }
  }
  public void Enter(Enemy enemy) {
    this.enemy = enemy;

    enemy.MyAnimator.SetBool("attack", false);
  }
  public void Exit() {

  }
  public void OnTriggerEnter(Collider2D other) {
    
  }

  private void CastProjectile() {
    throwTimer += Time.deltaTime;

    if (throwTimer >= throwCooldown) {
      canThrow = true;
    }

    if (throwTimer >= animationTimer) {
      enemy.MyAnimator.SetBool("throw", false);
    }

    if (canThrow && enemy.Target != null && enemy.InThrowRange) {
      canThrow = false;

      enemy.MyAnimator.SetBool("throw", true);

      throwTimer = 0;
    }
  }
}
