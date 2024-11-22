#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SkyboxPrint : MonoBehaviour
{
    [SerializeField]bool m_print;
    [SerializeField] private Cubemap cubeMapRef;

    private void Update()
    {
#if UNITY_EDITOR
        if (m_print)
        {
            GetComponent<Camera>().RenderToCubemap(cubeMapRef);
            //AssetDatabase.CreateAsset(cubeMap, "Assets\\Screenshots\\SkyboxPrint.asset");
            m_print = false;
        }
#endif
    }
}

