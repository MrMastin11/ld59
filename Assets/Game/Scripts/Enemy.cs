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

    private Vector2 moveDirection;
    private float timer;
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
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
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

        // одразу оновлюємо швидкість
        if (rb != null)
            rb.linearVelocity = moveDirection * moveSpeed;
    }

    // ====== СТІНИ ======
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            PickNewDirection();
        }
    }

    // ====== ГРАВЕЦЬ ======
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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