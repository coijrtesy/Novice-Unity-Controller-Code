using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleSate : IEnemyState
{
  private Enemy enemy;

  private float idleTimer;

  private float idleDuration = 5;
public void Execute() {
    Idle();

    if (enemy.Target != null) {
      enemy.ChangeState(new PatrolState());
    }
  }
  public void Enter(Enemy enemy) {
    this.enemy = enemy;

    enemy.MyAnimator.SetBool("throw", false);

    enemy.MyAnimator.SetBool("attack", false);
  }
  public void Exit() {

  }
  public void OnTriggerEnter(Collider2D other) {
    if (enemy.damageSources.Contains(other.tag) && ((enemy.transform.position.x - (Player.instance.transform.position.x + 2.5f) < 12) && (enemy.transform.position.x - (Player.instance.transform.position.x + 2.5f) > -12))) {
      enemy.Target = Player.Instance.gameObject;
    }
  }

  private void Idle() {
    enemy.MyAnimator.SetFloat("speed 1", 0);

    idleTimer += Time.deltaTime;

    if (idleTimer >= idleDuration) {
      enemy.ChangeState(new PatrolState());
    }
  }
}
