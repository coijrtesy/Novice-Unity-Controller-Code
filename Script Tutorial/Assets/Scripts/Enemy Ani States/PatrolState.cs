using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IEnemyState
{
  private Enemy enemy;
  
  private float patrolTimer;

  private float patrolDuration = 10;
public void Execute() {
    Patrol();

    enemy.Move();

    if (enemy.Target != null && enemy.InThrowRange) {
      enemy.ChangeState(new RangedSate());
    }
  }

  public void Enter(Enemy enemy) {
    this.enemy = enemy;
  }

  public void Exit() {

  }

  public void OnTriggerEnter(Collider2D other) {
    if (other.tag == "Edge") {
      enemy.ChangeDirectionTwo();
    }

    if (enemy.damageSources.Contains(other.tag) && ((enemy.transform.position.x - (Player.instance.transform.position.x + 2.5f) < 12) && (enemy.transform.position.x - (Player.instance.transform.position.x + 2.5f) > -12))) {
      enemy.Target = Player.Instance.gameObject;
    }
  }

  private void Patrol() {
    patrolTimer += Time.deltaTime;

    if (patrolTimer >= patrolDuration) {
      enemy.ChangeState(new IdleSate());
    }
  }
}
