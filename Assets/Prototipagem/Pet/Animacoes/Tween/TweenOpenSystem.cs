using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenOpenSystem : MonoBehaviour
{
    public RectTransform panelToOpen; 
    public float openHeight = 300f; 
    public float closedHeight = 0f; 
    public float openDuration = 0.5f; 
    public Tween.LerpType lerpType = Tween.LerpType.Lerp;

    void Start()
    {
        OpenPanel();
    }

    public void OpenPanel()
    {
        StartCoroutine(AnimatePanelSize(closedHeight, openHeight));
    }

    public void ClosePanel()
    {
        StartCoroutine(AnimatePanelSize(panelToOpen.sizeDelta.y, closedHeight));
    }

    private IEnumerator AnimatePanelSize(float startHeight, float endHeight)
    {
        

        float timer = 0f;
        Vector2 startSize = panelToOpen.sizeDelta;
        Vector2 finalSize = new Vector2(startSize.x, endHeight);

        while (timer < openDuration)
        {
            timer += Time.deltaTime;
            float t = timer / openDuration;

            // Interpolacao da altura do painel
            switch (lerpType)
            {
                case Tween.LerpType.Lerp:
                    panelToOpen.sizeDelta = new Vector2(startSize.x, Mathf.Lerp(startHeight, endHeight, t));
                    break;
               /* case Tween.LerpType.Slerp:
                    panelToOpen.sizeDelta = new Vector2(startSize.x, Mathf.Slerp(startHeight, endHeight, t));
                    break;*/
            }

            yield return null;
        }
        panelToOpen.sizeDelta = finalSize;
    }
}
