using UnityEngine;

public class BossPoisonOrb : MonoBehaviour
{
    [HideInInspector]public Rigidbody rb;
    [SerializeField] private PoisonPuddle poisonPuddle;
    [SerializeField] private Transform meshTransform;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void OnFire()
    {
        poisonPuddle.gameObject.SetActive(false);
        poisonPuddle.transform.parent = transform;
        poisonPuddle.transform.localPosition = Vector3.zero;
    }
    private void FixedUpdate()
    {
        meshTransform.LookAt(rb.position + rb.velocity);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if ((LayerManager.Instance.groundMask & (1 << collision.collider.gameObject.layer)) != 0)
        {
            poisonPuddle.transform.parent = null;
            poisonPuddle.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
