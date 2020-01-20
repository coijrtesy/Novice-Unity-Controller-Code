using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class MeleeAlternative : MonoBehaviour
{
  [SerializeField] private float speed;

  private Rigidbody2D myRigidbody;

  private Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
      myRigidbody = GetComponent<Rigidbody2D>();

      DestroySelf();
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

    void DestroySelf() {
      StartCoroutine(DestroyAfterDelay(0.05f));
    }

    private IEnumerator DestroyAfterDelay(float delay) {
      yield return new WaitForSeconds(delay);

      Destroy(gameObject);
    }
}