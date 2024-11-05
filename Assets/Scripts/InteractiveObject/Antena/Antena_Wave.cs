using UnityEngine;
using UnityEngine.Events;

public class Antena_Wave : MonoBehaviour
{
    public bool isTurnedOn { get; set; }
    public INeedRequirements connectedObject { get; set; }

    [SerializeField] private UnityEvent consequences;

    [Space(10), SerializeField] public MeshRenderer[] meshRenderers;

    [Space(10), SerializeField] private CableSpline[] cableSplines;

    void Awake()
    {
        Material material = new Material(meshRenderers[0].material);
        meshRenderers[0].sharedMaterial = material;
        meshRenderers[1].sharedMaterial = material;

    }
    public void TurnOn()
    {
        meshRenderers[0].sharedMaterial.SetInt("_Light_on_off", 1);
        foreach (CableSpline cableSpline in cableSplines)
        {
            cableSpline.ToggleSpline(true);
        }
        consequences.Invoke();
        if (connectedObject != null) connectedObject.OnRequirementChange();
    }
}
