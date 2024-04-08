using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Tween
{
    public enum LerpType
    {
        Lerp,
        Slerp
    }
    #region Move Transform
    /// <summary>
    /// Move a posicao do transform ate o ponto final no tempo determinado
    /// </summary>
    /// <param name="monoBehaviour">Onde a coroutine sera alocada pra funcionar</param>
    /// <param name="transform">Transform que sera movido</param>
    /// <param name="finalPosition">Ponto final da posicao do transform</param>
    /// <param name="duration">Tempo ate o transform chegar no seu ponto final</param>
    /// <returns>Retorna a corotina que esta manipulando a posicao do transform</returns>
    public static Coroutine MoveTransform(MonoBehaviour monoBehaviour, Transform transform, Vector3 finalPosition, float duration, LerpType lerpType)
    {
        return monoBehaviour.StartCoroutine(MoveTransform_Coroutine(transform, finalPosition, duration, lerpType));
    }
    private static IEnumerator MoveTransform_Coroutine(Transform transform, Vector3 finalPosition, float duration, LerpType lerpType)
    {
        float timer = 0;
        Vector3 startPosition = transform.position;
        while (timer < duration)
        {
            switch (lerpType)
            {
                case LerpType.Lerp:
                    transform.position = Vector3.Lerp(startPosition, finalPosition, timer / duration);
                    break;
                case LerpType.Slerp:
                    transform.position = Vector3.Slerp(startPosition, finalPosition, timer / duration);
                    break;
            }
                    timer += Time.deltaTime;
            yield return null;
        }
        transform.position = finalPosition;
    }
    #endregion

    #region Move Transform LocalPosition
    /// <summary>
    /// Move a posicao local do transform ate o ponto final no tempo determinado
    /// </summary>
    /// <param name="monoBehaviour">Onde a coroutine sera alocada pra funcionar</param>
    /// <param name="transform">Transform que sera movido</param>
    /// <param name="finalPosition">Ponto final da posicao do transform</param>
    /// <param name="duration">Tempo ate o transform chegar no seu ponto final</param>
    /// <returns>Retorna a corotina que esta manipulando a posicao do transform</returns>
    public static Coroutine MoveTransformLocalPosition(MonoBehaviour monoBehaviour, Transform transform, Vector3 finalPosition, float duration, LerpType lerpType)
    {
        return monoBehaviour.StartCoroutine(MoveTransformLocalPosition_Coroutine(transform, finalPosition, duration, lerpType));
    }
    private static IEnumerator MoveTransformLocalPosition_Coroutine(Transform transform, Vector3 finalPosition, float duration, LerpType lerpType)
    {
        float timer = 0;
        Vector3 startPosition = transform.localPosition;
        RectTransform canvasRectTransform = GetCanvasRectTransform(transform); // Obtém o RectTransform do Canvas
        Vector3 canvasLocalPosition = GetCanvasLocalPosition(canvasRectTransform, finalPosition); // Calcula a posição local do Canvas
        while (timer < duration)
        {
            switch (lerpType)
            {
                case LerpType.Lerp:
                    transform.position = Vector3.Lerp(startPosition, finalPosition, timer / duration);
                    break;
                case LerpType.Slerp:
                    transform.position = Vector3.Slerp(startPosition, finalPosition, timer / duration);
                    break;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = finalPosition;
    }
    #endregion

    private static RectTransform GetCanvasRectTransform(Transform transform)
    {
        Canvas canvas = transform.GetComponentInParent<Canvas>(); // Obtém o componente Canvas
        return canvas.GetComponent<RectTransform>(); // Retorna o RectTransform do Canvas
    }

    private static Vector3 GetCanvasLocalPosition(RectTransform canvasRectTransform, Vector3 screenPosition)
    {
        Vector2 canvasLocalPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPosition, Camera.main, out canvasLocalPosition);
        return canvasLocalPosition; // Retorna a posição local do Canvas
    }
    #region Rotate Transform
    /// <summary>
    /// Move a posicao local do transform ate o ponto final no tempo determinado
    /// </summary>
    /// <param name="monoBehaviour">Onde a coroutine sera alocada pra funcionar</param>
    /// <param name="transform">Transform que sera movido</param>
    /// <param name="finalPosition">Ponto final da posicao do transform</param>
    /// <param name="duration">Tempo ate o transform chegar no seu ponto final</param>
    /// <returns>Retorna a corotina que esta manipulando a posicao do transform</returns>
    public static Coroutine RotateTransform(MonoBehaviour monoBehaviour, Transform transform, Quaternion finalRotation, float duration, LerpType lerpType)
    {
        return monoBehaviour.StartCoroutine(RotateTransform_Coroutine(transform, finalRotation, duration, lerpType));
    }

    private static IEnumerator RotateTransform_Coroutine(Transform transform, Quaternion finalRotation, float duration, LerpType lerpType)
    {
        float timer = 0;
        Quaternion startRotation = transform.rotation;
        while (timer < duration)
        {
            switch (lerpType)
            {
                case LerpType.Lerp:
                    transform.rotation = Quaternion.Lerp(startRotation, finalRotation, timer / duration);
                    break;
                case LerpType.Slerp:
                    transform.rotation = Quaternion.Slerp(startRotation, finalRotation, timer / duration);
                    break;
            }
                    timer += Time.deltaTime;
            yield return null;
        }
        transform.rotation = finalRotation;
    }
    #endregion

    #region Move RectTransform LocalPosition
    /// <summary>
    /// Move a posicao do transform ate o ponto final no tempo determinado
    /// </summary>
    /// <param name="monoBehaviour">Onde a coroutine sera alocada pra funcionar</param>
    /// <param name="rectTransform">RectTransform que sera movido</param>
    /// <param name="finalPosition">Ponto final da posicao do transform</param>
    /// <param name="duration">Tempo ate o transform chegar no seu ponto final</param>
    /// <returns>Retorna a corotina que esta manipulando a posicao do transform</returns>
    public static Coroutine MoveRectTransform(MonoBehaviour monoBehaviour, RectTransform rectTransform, Vector2 finalPosition, float duration, LerpType lerpType)
    {
        return monoBehaviour.StartCoroutine(MoveRectTransform_Coroutine(rectTransform, finalPosition, duration, lerpType));
    }
    private static IEnumerator MoveRectTransform_Coroutine(RectTransform rectTransform, Vector2 finalPosition, float duration, LerpType lerpType)
    {
        float timer = 0;
        Vector3 startPosition = rectTransform.anchoredPosition;
        while (timer < duration)
        {
            switch (lerpType)
            {
                case LerpType.Lerp:
                    rectTransform.anchoredPosition = Vector3.Lerp(startPosition, finalPosition, timer / duration);
                    break;
                case LerpType.Slerp:
                    rectTransform.anchoredPosition = Vector3.Slerp(startPosition, finalPosition, timer / duration);
                    break;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = finalPosition;
    }
    #endregion
}
