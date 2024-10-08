using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmadilloUIControl : MonoBehaviour
{
    public static ArmadilloUIControl Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] private Image crossHair;
    [SerializeField] private Image hitMarker;
    private void Start()
    {
        hitMarker.color = new Color(hitMarker.color.r, hitMarker.color.g, hitMarker.color.b, 0);
    }
    #region
    public void StartHitMarker()
    {
        if (toggleHitMarker_Ref != null)
        {
            StopCoroutine(toggleHitMarker_Ref);
        }
        toggleHitMarker_Ref = StartCoroutine(ToggleHitMarker_Coroutine());
    }

    public Coroutine toggleHitMarker_Ref;
    private IEnumerator ToggleHitMarker_Coroutine()
    {
        hitMarker.color = new Color(hitMarker.color.r, hitMarker.color.g, hitMarker.color.b, 1);
        yield return new WaitForSeconds(0.1f);
        float timer = 0;
        float duration = 0.15f;
        while (timer < duration)
        {
            hitMarker.color = new Color(hitMarker.color.r, hitMarker.color.g, hitMarker.color.b, 1 - timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        hitMarker.color = new Color(hitMarker.color.r, hitMarker.color.g, hitMarker.color.b, 0);
    }
    #endregion

    #region Interaction
    [Space, Header("HUD")]
    public CanvasGroup interactHUD;
    public TextMeshProUGUI interactItemName;
    public TextMeshProUGUI interactKeybind;
    public TextMeshProUGUI interactDescription;
    #endregion
}
