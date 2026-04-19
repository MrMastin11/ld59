using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

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

    [Header("Audio")]
    public AudioClip[] sounds;
    private int currentSoundIndex = 0;

    [Header("Particles")]
    public ParticleSystem particlePrefab;

    [Header("Enemy")]
    public GameObject enemyPrefab;
    private int touchCounter = 0;
    private int n = 10;

    public Slider progressSlider;

    [Header("Progress")]
    public int maxTouches = 55;
    private int totalTouches = 0;

    [Header("Signal Positions")]
    public Vector3 startPosition = new Vector3(-2f, -2f, 0f);
    public Vector3 endPosition = new Vector3(10f, 10f, 0f);

    [Header("End Phase")]
    public float enemySpawnInterval = 2f;
    private bool isEndPhase = false;

    public static bool IsEndPhaseGlobal = false;

    private bool isStartPhase = true;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        tokens = ParseText(fullText);
        transform.position = startPosition;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isEndPhase) return;

        AddNextToken();
        SpawnParticles();

        touchCounter++;
        totalTouches++;

        UpdateProgressBar();

        if (touchCounter == n)
        {
            touchCounter = 0;
            if (n > 1)
                n--;

            SpawnEnemy();
        }

        if (totalTouches >= maxTouches)
        {
            StartEndPhase();
            return;
        }

        RandomSpawn();

        if (audioSource != null && sounds.Length > 0)
        {
            audioSource.PlayOneShot(sounds[currentSoundIndex]);

            currentSoundIndex++;

            if (currentSoundIndex >= sounds.Length)
                currentSoundIndex = 0;
        }
    }

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

                result.Add(";");
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

        if (token == ";")
        {
            ComputerText.text = "";
            currentIndex++;

            if (currentIndex >= tokens.Length) return;

            token = tokens[currentIndex];
        }

        if (ComputerText.text.Length > 0)
            ComputerText.text += " ";

        ComputerText.text += token;

        currentIndex++;
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        float x = Random.Range(-6.8f, -0.5f);
        float y = Random.Range(-1.3f, 3.3f);

        Instantiate(enemyPrefab, new Vector3(x, y, 0), Quaternion.identity);
    }

    void SpawnParticles()
    {
        if (particlePrefab == null) return;

        ParticleSystem ps = Instantiate(particlePrefab, transform.position, Quaternion.identity);
        ps.Play();

        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
    }

    public void RandomSpawn()
    {
        if (isStartPhase)
        {
            isStartPhase = false;
            transform.position = startPosition;
        }
        else
        {
            float x = Random.Range(-6.8f, -0.5f);
            float y = Random.Range(-1.3f, 3.3f);
            transform.position = new Vector3(x, y, 0);
        }
    }

    void UpdateProgressBar()
    {
        if (progressSlider == null) return;
        progressSlider.value = totalTouches;
    }

    void StartEndPhase()
    {
        isEndPhase = true;
        IsEndPhaseGlobal = true;

        transform.position = endPosition;

        InvokeRepeating(nameof(SpawnEnemy), 0f, enemySpawnInterval);
    }
}
