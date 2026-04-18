using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class SignalZone : MonoBehaviour
{
    public TextMeshProUGUI ComputerText;

    [TextArea]
    public string fullText = "Hello Earthlings";

    private int currentIndex = 0;

    private AudioSource audioSource;

    [Header("Particles")]
    public ParticleSystem particlePrefab;

    [Header("Enemy")]
    public GameObject enemyPrefab;
    private int touchCounter = 0;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        AddNextLetter();
        SpawnParticles();

        touchCounter++;

        if (touchCounter % 3 == 0)
        {
            SpawnEnemy();
        }

        RandomSpawn();
        audioSource.Play();
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        float x = Random.Range(-6.8f, -0.5f);
        float y = Random.Range(-1.3f, 3.3f);

        Vector3 pos = new Vector3(x, y, 0);

        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }

    void AddNextLetter()
    {
        if (currentIndex < fullText.Length)
        {
            ComputerText.text += fullText[currentIndex];
            currentIndex++;
        }
    }

    void SpawnParticles()
    {
        if (particlePrefab == null) return;

        ParticleSystem ps = Instantiate(
            particlePrefab,
            transform.position,
            Quaternion.identity
        );

        ps.Play();

        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
    }

    public void RandomSpawn()
    {
        float x = Random.Range(-6.8f, -0.5f);
        float y = Random.Range(-1.3f, 3.3f);

        transform.position = new Vector3(x, y, 0);
    }
}