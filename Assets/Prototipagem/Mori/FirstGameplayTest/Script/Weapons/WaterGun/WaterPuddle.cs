using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WaterPuddle : MonoBehaviour
{
    [SerializeField] private float size;
    private float realSize;
    DecalProjector decalProjector;
    BoxCollider boxCollider;
    private void OnValidate()
    {
        #if UNITY_EDITOR
        decalProjector = GetComponent<DecalProjector>();
        boxCollider = GetComponent<BoxCollider>();
        decalProjector.size = new Vector3(size, size, decalProjector.size.z);
        boxCollider.size = new Vector3(size, size, boxCollider.size.z);
        #endif
    }
    private void Awake()
    {
        realSize = size;
        decalProjector = GetComponent<DecalProjector>();
        boxCollider = GetComponent<BoxCollider>();
    }
    public void ChangeSize(float deltaSize)
    {
        realSize =realSize +deltaSize;
        if(changeSize_Ref!=null)StopCoroutine(changeSize_Ref);
        changeSize_Ref = StartCoroutine(ChangeSize_Coroutine(realSize));
    }
    public Coroutine changeSize_Ref;
    public IEnumerator ChangeSize_Coroutine(float newSize)
    {
        float timer = 0;
        float duration = 0.5f;
        float lerpSize;
        while (timer < duration)
        {
            lerpSize = Mathf.Lerp(size, newSize, timer / duration);
            size = lerpSize;
            decalProjector.size = new Vector3(lerpSize, lerpSize, decalProjector.size.z);
            boxCollider.size = new Vector3(lerpSize, lerpSize, boxCollider.size.z);
            timer += 0.015f;
            yield return new WaitForSeconds(0.015f);
        }
    }
}
