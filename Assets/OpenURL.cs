using UnityEngine;

public class OpenURL : MonoBehaviour
{
    public void OpenMyLink(string url)
    {
        Application.OpenURL(url);
    }
}
