using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SignalZone : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI ComputerText;

    [TextArea]
    public string fullText = "Hello Earthlings; We come in peace";

    private int currentIndex = 0;
    private string[] tokens;

    private AudioSource audioSource;

    [Header("Particles")]
    public ParticleSystem particlePrefab;

    [Header("Enemy")]
    public GameObject enemyPrefab;
    private int touchCounter = 0;
    private int n = 7;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        tokens = ParseText(fullText);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        AddNextToken();
        SpawnParticles();

        touchCounter++;

        if (touchCounter == n)
        {
            touchCounter = 0;
            if (n > 1)
                n--;

            SpawnEnemy();
        }

        RandomSpawn();
        audioSource.Play();
    }

    // ===================== TEXT LOGIC =====================

    string[] ParseText(string text)
    {
        List<string> result = new List<string>();
        string currentWord = "";

        foreach (char c in text)
        {
            if (c == ' ')
            {
                if (currentWord != "")
                {
                    result.Add(currentWord);
                    currentWord = "";
                }
            }
            else if (c == ';')
            {
                if (currentWord != "")
                {
                    result.Add(currentWord);
                    currentWord = "";
                }

                result.Add(";"); // команда очистки
            }
            else
            {
                currentWord += c;
            }
        }

        if (currentWord != "")
            result.Add(currentWord);

        return result.ToArray();
    }

    void AddNextToken()
    {
        if (currentIndex >= tokens.Length) return;

        string token = tokens[currentIndex];

        // Якщо це команда очистки
        if (token == ";")
        {
            ComputerText.text = "";
            currentIndex++;

            // Після очистки одразу беремо наступне слово
            if (currentIndex >= tokens.Length) return;

            token = tokens[currentIndex];
        }

        // Додаємо слово
        if (ComputerText.text.Length > 0)
            ComputerText.text += " ";

        ComputerText.text += token;

        currentIndex++;
    }

    // ===================== GAME LOGIC =====================

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        float x = Random.Range(-6.8f, -0.5f);
        float y = Random.Range(-1.3f, 3.3f);

        Vector3 pos = new Vector3(x, y, 0);

        Instantiate(enemyPrefab, pos, Quaternion.identity);
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