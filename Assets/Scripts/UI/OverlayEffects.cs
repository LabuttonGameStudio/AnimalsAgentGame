using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayEffects : MonoBehaviour
{
    public static OverlayEffects Instance;
    public Image sandOverlay;
    public Image poisonOverlay;

    private void Awake()
    {
        Instance = this;
    }

    public void ToggleImage(Image image,bool toggle)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b,toggle ? 1 : 0);
    }
}
