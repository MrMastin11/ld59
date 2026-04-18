using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Spawn")]
    public float minX = -6.8f;
    public float maxX = -0.5f;
    public float minY = -1.3f;
    public float maxY = 2.5f;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float changeDirectionTime = 1f;

    [Header("Particles")]
    public GameObject particlePrefab;
    public float particleInterval = 0.5f;
    public float particleLifetime = 2f;

    private Vector2 moveDirection;
    private float timer;
    private float particleTimer;
    private Rigidbody2D rb;

    // ====== SPAWN ======
    public static void Spawn(GameObject prefab)
    {
        if (prefab == null) return;

        float x = Random.Range(-6.8f, -0.5f);
        float y = Random.Range(-1.3f, 2.5f);

        Vector3 pos = new Vector3(x, y, 0);

        Instantiate(prefab, pos, Quaternion.identity);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        PickNewDirection();
    }

    void Update()
    {
        HandleDirectionChange();
        HandleParticles();
    }

    void FixedUpdate()
    {
        Vector2 nextPos = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);
    }

    void HandleDirectionChange()
    {
        timer += Time.deltaTime;

        if (timer >= changeDirectionTime)
        {
            PickNewDirection();
            timer = 0f;
        }
    }

    void PickNewDirection()
    {
        moveDirection = Random.insideUnitCircle.normalized;

        if (rb != null)
            rb.linearVelocity = moveDirection * moveSpeed;
    }

    // ====== PARTICLES ======
    void HandleParticles()
    {
        if (particlePrefab == null) return;

        particleTimer += Time.deltaTime;

        if (particleTimer >= particleInterval)
        {
            SpawnParticles();
            particleTimer = 0f;
        }
    }

    void SpawnParticles()
    {
        GameObject p = Instantiate(particlePrefab, transform.position, Quaternion.identity);

        Destroy(p, particleLifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            PickNewDirection();
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Screamer();
            Destroy(gameObject);
        }
    }

    public void Screamer()
    {
        Debug.Log("Enemy Screams!");
    }
}