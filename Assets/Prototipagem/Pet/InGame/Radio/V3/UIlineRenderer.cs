using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIlineRenderer : Graphic
{
    public int points = 100; //  pontos na linha
    public float amplitude = 50f; // altura da onda
    public float frequency = 1f; // frequencia da onda
    public float speed = 1f; // velocidade da onda
    public float randomness = 10f; // aleatoriedade
    public float thickness = 5f; //espessura

    private float offset = 0f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        offset += Time.deltaTime * speed;
        List<Vector2> wavePoints = new List<Vector2>();

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        for (int i = 0; i < points; i++)
        {
            float x = (float)i / (points - 1) * width;
            float y = Mathf.Sin(x * frequency + offset) * amplitude + Random.Range(-randomness, randomness);
            wavePoints.Add(new Vector2(x, height / 2 + y));
        }

        for (int i = 0; i < wavePoints.Count - 1; i++)
        {
            AddLine(vh, wavePoints[i], wavePoints[i + 1]);
        }
    }

    private void AddLine(VertexHelper vh, Vector2 pointA, Vector2 pointB)
    {
        Vector2 direction = (pointB - pointA).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x) * thickness / 2;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = pointA - perpendicular;
        vh.AddVert(vertex);

        vertex.position = pointA + perpendicular;
        vh.AddVert(vertex);

        vertex.position = pointB + perpendicular;
        vh.AddVert(vertex);

        vertex.position = pointB - perpendicular;
        vh.AddVert(vertex);

        int startIndex = vh.currentVertCount - 4;
        vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
        vh.AddTriangle(startIndex, startIndex + 2, startIndex + 3);
    }

    void Update()
    {
        SetVerticesDirty();
    }
}
