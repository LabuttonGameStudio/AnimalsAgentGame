using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramGlitch : MonoBehaviour
{
    public float maxGlitch;

    public float minGlitch;

    public float glitchLength;

    public float timeBetweenGlitches;

    private void Start()
    {
        StartCoroutine(Glitch());
    }

    IEnumerator Glitch()
    {
        Renderer renderer = GetComponent<Renderer>();

        Material mainMaterial = renderer.material; // Referencia do componente que vai ser modificado

        yield return new WaitForSeconds(timeBetweenGlitches); // Tempo entre os glitches

        mainMaterial.SetFloat("_Glitch_Strength", Random.Range(minGlitch, maxGlitch)); // Pega referencia da string existente dentro do shader

        yield return new WaitForSeconds(glitchLength); // Quantidade de glitches dentro do tempo especificado

        mainMaterial.SetFloat("_Glitch_Strength", 0f);

        StartCoroutine(Glitch());
    }

}
