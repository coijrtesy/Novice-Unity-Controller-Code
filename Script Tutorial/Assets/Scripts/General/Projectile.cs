using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Projectile : MonoBehaviour
{
  [SerializeField] private float speed;

  private Rigidbody2D myRigidbody;

  private Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
      myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize(Vector2 direction) {
      this.direction = direction;
    }

    void FixedUpdate() {
      myRigidbody.velocity = direction * speed;
    }

    void OnBecameInvisible() {
      Destroy(gameObject);
    }
}
