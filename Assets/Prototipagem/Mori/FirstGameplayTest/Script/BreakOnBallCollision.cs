using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BreakOnBallCollision : MonoBehaviour
{
    Collider m_collider;
    MeshRenderer m_meshRenderer;

    [SerializeField] private VisualEffect onDestroyVFX;
    [SerializeField] private ParticleSystem puffParticle;
    private void Awake()
    {
        m_collider = GetComponent<Collider>();
        m_meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            ArmadilloPlayerController playerControler = ArmadilloPlayerController.Instance;
            if (playerControler.currentForm == ArmadilloPlayerController.Form.Ball)
            {
                if (collision.impulse.magnitude > 200)
                {
                    if(onDestroyVFX != null) onDestroyVFX.Play();
                    if(puffParticle != null) puffParticle.Play();
                    playerControler.visualControl.OnBallHit(collision.GetContact(0).point,playerControler.transform.position);
                    m_collider.enabled = false;
                    m_meshRenderer.enabled = false;
                    StartCoroutine(ApplySpeedToPlayer(playerControler.movementControl));
                }
            }
        }
    }
    private IEnumerator ApplySpeedToPlayer(ArmadilloMovementController movementController)
    {
        yield return new WaitForFixedUpdate();
        movementController.OnBreakObject();
    }    

}
