using UnityEngine;

public class SkyboxPrint : MonoBehaviour
{
    [SerializeField]bool m_print;
    [SerializeField] int resolution;
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (m_print)
        {
            ScreenCapture.CaptureScreenshot("Assets\\Screenshots\\" + "PrintSkybox.png", resolution);
            m_print = false;
        }
    }
#endif
}
