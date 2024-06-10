using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HologramGlitchImage : MonoBehaviour
{
    [Header("GLITCH")]
    public float maxGlitch;
    public float minGlitch;

    public float glitchLength;
    public float timeBetweenGlitches;

    [Header("SHINE")]
    public float maxShine;
    public float minShine;

    public float timeBetweenShine;

    private void Start()
    {
        StartCoroutine(Shine());
        StartCoroutine(Glitch());
    }

    private void Update()
    {
        StartCoroutine(Shine());
        StartCoroutine(Glitch());
    }

    IEnumerator Glitch()
    {
        Image image = GetComponent<Image>();

        Material mainMaterial = image.material; // Referencia do componente que vai ser modificado

        while (true)
        {
            yield return new WaitForSeconds(timeBetweenGlitches); // Tempo entre os glitches

            mainMaterial.SetFloat("_Glitch_Strength", Random.Range(minGlitch, maxGlitch)); // Pega referencia da string existente dentro do shader

            yield return new WaitForSeconds(glitchLength); // Quantidade de glitches dentro do tempo especificado

            mainMaterial.SetFloat("_Glitch_Strength", 0f);
        }
    }

    IEnumerator Shine()
    {
        Image image = GetComponent<Image>();

        Material mainMaterial = image.material;

        while (true)
        {
            // Aumenta
            yield return ChangeShine(mainMaterial, minShine, maxShine, timeBetweenShine);

            // Diminui
            yield return ChangeShine(mainMaterial, maxShine, minShine, timeBetweenShine);
        }
    }

    IEnumerator ChangeShine(Material material, float startShine, float endShine, float duration)
    {
        float elapsedTime = 0f;
        Color baseColor = material.GetColor("_Color") / Mathf.Max(material.GetColor("_Color").maxColorComponent, 1); // Normalize

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float intensity = Mathf.Lerp(startShine, endShine, elapsedTime / duration);
            material.SetColor("_Color", baseColor * intensity);
            yield return null;
        }

        material.SetColor("_Color", baseColor * endShine); // Modificar o intensity do HDR apenas
    }


}
