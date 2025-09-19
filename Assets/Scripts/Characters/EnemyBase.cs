using UnityEngine;
public enum EnemyState
{
    Pooled,
    Play,
    Freeze,
    Die,
}
public class EnemyBase : MonoBehaviour
{

    protected EnemyState _currentState = EnemyState.Pooled;
    public float Speed = 1; // Units Per second
    public float Radius = 10;
    public static int CollisionLayer = 2;
    private float sqr_rad = 0;
    protected int dir = 1;
    private Vector2 _spawnPoint = Vector2.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected Rigidbody2D rigidbody2D;
    protected Collider2D collider2D;
    protected void Die()
    {
        //Play animation if any
        _currentState = EnemyState.Die;
        Debug.Log($"Goomba instance {gameObject.GetInstanceID()} Died!");

    }
    public void Spawn(Vector2 spawn_point)
    {
        this.rigidbody2D.position = spawn_point;
        this.enabled = false;
        _currentState = EnemyState.Freeze;
        _spawnPoint = spawn_point;
    }
    public void Enable()
    {
        this.enabled = true;
        _currentState = EnemyState.Play;
        rigidbody2D.linearVelocityX = Speed;
        //Start any animation stuff as well;
    }
    protected void Awake()
    {
        GameManager.Instance.OnGameStart += Enable;
    }
    public void OnDestroy()
    {
        GameManager.Instance.OnGameStart -= Enable;
    }
    protected void Start()
    {
        Debug.Log("Goomba started");
        collider2D = this.gameObject.GetComponent<Collider2D>();
        rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        if (collider2D == null) Debug.LogError("Did not get collider");
        if (rigidbody2D == null) Debug.LogError("Did not get rigidbody");
        collider2D.excludeLayers = CollisionLayer;
        _spawnPoint = rigidbody2D.position;
        sqr_rad = Radius * Radius;
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        if (_currentState == EnemyState.Play)
        {
            var delta_vec2 = rigidbody2D.position - _spawnPoint;
            if (delta_vec2.sqrMagnitude > sqr_rad)
            {
                dir = delta_vec2.x >= 0 ? -1 : 1;
                Debug.Log("Goomba going back to spawn point");
            }
             rigidbody2D.linearVelocityX = dir * Speed;
        }
    }
    protected void FlipDirection()
    {
        if (rigidbody2D == null) return;
        dir = -dir;
        rigidbody2D.linearVelocityX = dir * Speed;
    }
    
}
