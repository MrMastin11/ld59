using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
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

    [Header("Trail Particles")]
    public GameObject particlePrefab;
    public float particleInterval = 0.1f;
    public float particleLifetime = 1.5f;

    [Header("Camera Shake")]
    public float shakeDuration = 1.2f;
    public float shakeIntensity = 1.5f;

    [Header("UI Shake")]
    public float uiShakeIntensity = 10f;

    [Header("Audio")]
    public AudioClip hitSound;

    [Header("End Collision")]
    public GameObject endHitParticles;
    public float loadDelay = 10f;

    private bool isEndingTriggered = false;

    private Vector2 moveDirection;
    private float timer;
    private float particleTimer;

    private Rigidbody2D rb;
    private AudioSource audioSource;

    private Transform cam;
    private Vector3 camStartPos;

    private RectTransform[] uiTargets;
    private Vector2[] uiStartPositions;

    private SpriteSwitcher spriteSwitcher;
    private MovingSineWave wave;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        PickNewDirection();

        spriteSwitcher = FindObjectOfType<SpriteSwitcher>();
        wave = FindObjectOfType<MovingSineWave>();

        if (Camera.main != null)
        {
            cam = Camera.main.transform;
            camStartPos = cam.position;
        }

        GameObject[] uiObjects = GameObject.FindGameObjectsWithTag("UI_ShakeTarget");

        uiTargets = new RectTransform[uiObjects.Length];
        uiStartPositions = new Vector2[uiObjects.Length];

        for (int i = 0; i < uiObjects.Length; i++)
        {
            uiTargets[i] = uiObjects[i].GetComponent<RectTransform>();

            if (uiTargets[i] != null)
                uiStartPositions[i] = uiTargets[i].anchoredPosition;
        }
    }

    void Update()
    {
        HandleDirectionChange();
        HandleTrail();
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
    }

    void HandleTrail()
    {
        if (particlePrefab == null) return;

        particleTimer += Time.deltaTime;

        if (particleTimer >= particleInterval)
        {
            GameObject p = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            Destroy(p, particleLifetime);
            particleTimer = 0f;
        }
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
            // ‘≤Õ¿À‹Õ¿ ‘¿«¿
            if (SignalZone.IsEndPhaseGlobal && !isEndingTriggered)
            {
                isEndingTriggered = true;

                if (endHitParticles != null)
                {
                    GameObject p = Instantiate(endHitParticles, transform.position, Quaternion.identity);

                    ParticleSystem ps = p.GetComponentInChildren<ParticleSystem>();

                    if (ps != null)
                    {
                        ps.Play();
                        Destroy(p, ps.main.duration + ps.main.startLifetime.constantMax);
                    }
                    else
                    {
                        Debug.LogWarning("No ParticleSystem found on endHitParticles!");
                        //Destroy(p, 5f);
                    }

                }

                StartCoroutine(LoadEndScene());
                return;
            }

            // «¬»◊¿…Õ¿ ÀŒ√≤ ¿
            if (audioSource != null && hitSound != null)
                audioSource.PlayOneShot(hitSound);

            if (spriteSwitcher != null)
                spriteSwitcher.SetHitSprite();

            if (wave != null)
                wave.TriggerNoiseBoost();

            StartCoroutine(ShakeBoth());
        }
    }

    IEnumerator LoadEndScene()
    {
        yield return new WaitForSeconds(loadDelay);
        SceneManager.LoadScene("Untitled");
    }

    IEnumerator ShakeBoth()
    {
        float t = 0f;

        while (t < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            if (cam != null)
                cam.position = camStartPos + new Vector3(x, y, 0);

            if (uiTargets != null)
            {
                for (int i = 0; i < uiTargets.Length; i++)
                {
                    if (uiTargets[i] != null)
                    {
                        uiTargets[i].anchoredPosition =
                            uiStartPositions[i] + new Vector2(x * uiShakeIntensity, y * uiShakeIntensity);
                    }
                }
            }

            t += Time.deltaTime;
            yield return null;
        }

        if (cam != null)
            cam.position = camStartPos;

        if (uiTargets != null)
        {
            for (int i = 0; i < uiTargets.Length; i++)
            {
                if (uiTargets[i] != null)
                    uiTargets[i].anchoredPosition = uiStartPositions[i];
            }
        }

        if (spriteSwitcher != null)
            spriteSwitcher.ResetSprite();
    }
}
