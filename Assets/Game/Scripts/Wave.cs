using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(AudioSource))]
public class MovingSineWave : MonoBehaviour
{
    [Header("Wave settings")]
    public int points = 200;
    public float amplitude = 1f;
    public float frequency = 1f;
    public float speed = 2f;
    public float length = 10f;
    public Vector3 startPosition = Vector3.zero;

    [Header("Noise settings")]
    public float maxNoise = 5f;
    public float minNoise = 0f;
    public float maxDistance = 5f;

    [Header("Hit Noise Boost")]
    public float hitNoiseBoost = 0.5f;      // різкий буст
    public float hitNoiseDuration = 0.5f;   // час повернення

    private float currentNoiseBoost = 0f;
    private float noiseVelocity = 0f;

    [Header("Sound settings")]
    public float maxVolume = 1f;
    public float minVolume = 0f;

    [Header("References")]
    public Transform player;
    public SignalZone signalZone;
    public GameObject StartButton;
    public GameObject Player;
    public TMPro.TextMeshProUGUI ComputerText;

    private LineRenderer lr;
    private AudioSource audioSource;
    private bool isOnScene = false;

    [Header("Start sound")]
    public AudioClip startSound;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();

        lr.positionCount = points;
        lr.useWorldSpace = false;

        ComputerText.text = "Unknown signal";
        player.gameObject.SetActive(false);

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 1f;
    }

    public void StartWave()
    {
        isOnScene = true;

        StartButton.GameObject().SetActive(false);
        ComputerText.text = "W A S D\nUse Wave\nTo Find Signal";

        signalZone.RandomSpawn();
        player.gameObject.SetActive(true);

        // старт звук
        if (startSound != null)
            audioSource.PlayOneShot(startSound);

        // основний звук
        audioSource.Play();
    }

    //  виклик при ударі
    public void TriggerNoiseBoost()
    {
        currentNoiseBoost = hitNoiseBoost;
    }

    void Update()
    {
        if (!isOnScene || player == null || signalZone == null) return;

        // плавне затухання бусту
        currentNoiseBoost = Mathf.SmoothDamp(
            currentNoiseBoost,
            0f,
            ref noiseVelocity,
            hitNoiseDuration
        );

        float timeOffset = Time.time * speed;
        float distance = Vector3.Distance(player.position, signalZone.transform.position);

        float t = Mathf.Clamp01(distance / maxDistance);
        t = t * t;

        // базовий шум + буст
        float baseNoise = Mathf.Lerp(maxNoise, minNoise, t);
        float noiseAmount = baseNoise + currentNoiseBoost;

        // товщина
        lr.startWidth = 0.1f + Mathf.Sin(Time.time * 5f) * 0.02f;
        lr.endWidth = lr.startWidth;

        // звук
        float volume = Mathf.Lerp(maxVolume, minVolume, t);
        audioSource.volume = volume;
        audioSource.pitch = Mathf.Lerp(0.5f, 1.5f, 1 - t);

        for (int i = 0; i < points; i++)
        {
            float tPoint = (float)i / (points - 1);
            float x = tPoint * length;

            float y = Mathf.Sin(x * frequency - timeOffset) * amplitude;
            y += Random.Range(-noiseAmount, noiseAmount);

            lr.SetPosition(i, startPosition + new Vector3(x, y, 0));
        }
    }
}