using UnityEngine;
using UnityEngine.SceneManagement;

public class SpriteSwitcher : MonoBehaviour
{
    public Sprite normalSprite;
    public Sprite hitSprite;

    private SpriteRenderer sr;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = normalSprite;
    }

    public void SetHitSprite()
    {
        sr.sprite = hitSprite;
    }

    public void ResetSprite()
    {
        sr.sprite = normalSprite;
    }
}
