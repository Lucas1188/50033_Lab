using UnityEngine;

public class KnockBoxBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    public float impulseForce =2;
    public float minTriggerInterval=0.5f;
    private float lastKnockTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Collision box");
        if (collision.gameObject.tag == "PlayerTriggerBox")
        {
            // Debug.Log($"Collision Box:{rb.gameObject.tag} {rb.position} Player:{collision.gameObject.tag} {collision.attachedRigidbody.position}");
            if (Vector3.Dot(Vector3.up, (rb.position - collision.attachedRigidbody.position).normalized) > 0 && Time.time > lastKnockTime + minTriggerInterval)
            {
                //Debug.LogWarning($"Box.Player {Vector3.Dot(Vector3.up, (rb.position - collision.attachedRigidbody.position).normalized)}");
                rb.AddForce(Vector2.up * impulseForce, ForceMode2D.Impulse);
                lastKnockTime = Time.time;
            }
        }
    }
}
