using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    void Awake()
    {
        // Fixa o framerate em 60 FPS
        Application.targetFrameRate = 60;
        
        // Desativa VSync para ter controle total
        QualitySettings.vSyncCount = 0;
    }
}