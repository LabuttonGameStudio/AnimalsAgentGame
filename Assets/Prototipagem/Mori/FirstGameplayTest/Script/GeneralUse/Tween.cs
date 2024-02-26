using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Tween
{
    /// <summary>
    /// Move a posicao do transform ate o ponto final no tempo determinado
    /// </summary>
    /// <param name="monoBehaviour">Onde a coroutine sera alocada pra funcionar</param>
    /// <param name="transform">Transform que sera movido</param>
    /// <param name="finalPosition">Ponto final da posicao do transform</param>
    /// <param name="duration">Tempo ate o transform chegar no seu ponto final</param>
    /// <returns>Retorna a corotina que esta manipulando a posicao do transform</returns>
    public static Coroutine MoveTransform(MonoBehaviour monoBehaviour, Transform transform, Vector3 finalPosition, float duration)
    {
        return monoBehaviour.StartCoroutine(MoveTransform_Coroutine(transform, finalPosition, duration));
    }
    private static IEnumerator MoveTransform_Coroutine(Transform transform, Vector3 finalPosition, float duration)
    {
        float timer = 0;
        Vector3 startPosition = transform.position;
        while (timer < duration)
        {
            transform.position = Vector3.Lerp(startPosition, finalPosition, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = finalPosition;
    }
    /// <summary>
    /// Move a posicao local do transform ate o ponto final no tempo determinado
    /// </summary>
    /// <param name="monoBehaviour">Onde a coroutine sera alocada pra funcionar</param>
    /// <param name="transform">Transform que sera movido</param>
    /// <param name="finalPosition">Ponto final da posicao do transform</param>
    /// <param name="duration">Tempo ate o transform chegar no seu ponto final</param>
    /// <returns>Retorna a corotina que esta manipulando a posicao do transform</returns>
    public static Coroutine MoveTransformLocalPosition(MonoBehaviour monoBehaviour, Transform transform, Vector3 finalPosition, float duration)
    {
        return monoBehaviour.StartCoroutine(MoveTransformLocalPosition_Coroutine(transform, finalPosition, duration));
    }
    private static IEnumerator MoveTransformLocalPosition_Coroutine(Transform transform, Vector3 finalPosition, float duration)
    {
        float timer = 0;
        Vector3 startPosition = transform.localPosition;
        while (timer < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, finalPosition, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = finalPosition;
    }
}
