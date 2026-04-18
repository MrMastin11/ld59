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
    public float maxNoise = 5f;   // далеко (шумно)
    public float minNoise = 0f;   // близько (чисто)
    public float maxDistance = 5f;

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

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();

        lr.positionCount = points;
        lr.useWorldSpace = false;

        ComputerText.text = "Warning From Deep Space";
        player.gameObject.SetActive(false);

        // Налаштування звуку
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
    }

    public void StartWave()
    {
        isOnScene = true;

        StartButton.GameObject().SetActive(false);
        ComputerText.text = "w a s d";

        signalZone.RandomSpawn();
        player.gameObject.SetActive(true);

        audioSource.Play();
    }

    void Update()
    {
        if (!isOnScene || player == null || signalZone == null) return;

        float timeOffset = Time.time * speed;

        float distance = Vector3.Distance(player.position, signalZone.transform.position);

        float t = Mathf.Clamp01(distance / maxDistance);
        t = t * t; // плавніше

        // ШУМ
        float noiseAmount = Mathf.Lerp(maxNoise, minNoise, t);

        // ТОВЩИНА ЛІНІЇ
        lr.startWidth = 0.1f + Mathf.Sin(Time.time * 5f) * 0.02f;
        lr.endWidth = lr.startWidth;

        // 🔊 ГУЧНІСТЬ (чим ближче — тим гучніше)
        float volume = Mathf.Lerp(maxVolume, minVolume, t);
        volume = Mathf.Pow(volume, 1f); // більш природно
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