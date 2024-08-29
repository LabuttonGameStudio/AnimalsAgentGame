using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WaterPuddle : MonoBehaviour
{
    public float size;
    [HideInInspector]public float realSize;
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
    public void UpdateSize(float newSize)
    {
        size = newSize;
        realSize = size;
        decalProjector.size = new Vector3(newSize, newSize, decalProjector.size.z);
        boxCollider.size = new Vector3(newSize, newSize, boxCollider.size.z);
    }
    public void ChangeSize(float newSize)
    {
        realSize = newSize;
        if(changeSize_Ref!=null)StopCoroutine(changeSize_Ref);
        changeSize_Ref = StartCoroutine(ChangeSize_Coroutine());
    }
    public void AddSize(float deltaSize)
    {
        realSize = realSize + deltaSize;
        if (changeSize_Ref != null) StopCoroutine(changeSize_Ref);
        changeSize_Ref = StartCoroutine(ChangeSize_Coroutine());
    }
    public Coroutine changeSize_Ref;
    public IEnumerator ChangeSize_Coroutine()
    {
        float timer = 0;
        float duration = 0.3f;
        float lerpSize;
        while (timer < duration)
        {
            lerpSize = Mathf.Lerp(size, realSize, timer / duration);
            size = lerpSize;
            decalProjector.size = new Vector3(size, size, decalProjector.size.z);
            boxCollider.size = new Vector3(size, size, boxCollider.size.z);
            timer += 0.025f;
            yield return new WaitForSeconds(0.025f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Puddle"))
        {
            WaterGunProjectileManager.Instance.TryMergePuddle(this, other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
    }
}
