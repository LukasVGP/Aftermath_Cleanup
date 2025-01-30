using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    void Awake()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        Application.targetFrameRate = 60;
    }
}
