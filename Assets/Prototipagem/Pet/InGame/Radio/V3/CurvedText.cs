using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurvedText : MonoBehaviour
{
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public float curveScale = 10f;

    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        UpdateTextMesh();
    }

    void Update()
    {
        
        UpdateTextMesh();
    }

    void UpdateTextMesh()
    {
        textMeshPro.ForceMeshUpdate();

        var textInfo = textMeshPro.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            var verts = textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].vertices;
            for (int j = 0; j < 4; j++)
            {
                var orig = verts[textInfo.characterInfo[i].vertexIndex + j];
                var normalizedX = orig.x / textMeshPro.rectTransform.rect.width;
                var curveValue = curve.Evaluate(normalizedX) * curveScale;
                verts[textInfo.characterInfo[i].vertexIndex + j] = new Vector3(orig.x, orig.y + curveValue, orig.z);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textMeshPro.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}

