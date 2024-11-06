using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ContainerSpawner : MonoBehaviour
{
    [SerializeField] private Transform doorFront_L;
    [SerializeField] private Transform doorFront_R;
    [SerializeField] private Transform doorBack_L;
    [SerializeField] private Transform doorBack_R;

    [SerializeField] private NavMeshObstacle[] navMeshFrontObstacle;
    [SerializeField] private NavMeshObstacle[] navMeshBackObstacle;

    enum DoorSideEnum
    {
        Front,
        Both
    }
    [SerializeField] private DoorSideEnum doorOpenSide;
    public void Open()
    {
        StartCoroutine(OpenDoors_Coroutine());
    }

    private IEnumerator OpenDoors_Coroutine()
    {
        switch (doorOpenSide)
        {
            case DoorSideEnum.Front:
                StartCoroutine(LerpAngle_Coroutine(doorFront_L, 90));
                yield return StartCoroutine(LerpAngle_Coroutine(doorFront_R, -90));
                foreach (NavMeshObstacle obstacle in navMeshFrontObstacle)
                {
                    obstacle.enabled = false;
                }
                break;
            case DoorSideEnum.Both:
                StartCoroutine(LerpAngle_Coroutine(doorFront_L, 90));
                StartCoroutine(LerpAngle_Coroutine(doorFront_R, -90));

                StartCoroutine(LerpAngle_Coroutine(doorBack_L, -90));
                yield return StartCoroutine(LerpAngle_Coroutine(doorBack_R, 90));

                foreach (NavMeshObstacle obstacle in navMeshFrontObstacle)
                {
                    obstacle.enabled = false;
                }
                foreach (NavMeshObstacle obstacle in navMeshBackObstacle)
                {
                    obstacle.enabled = false;
                }
                break;
        }
    }
    private IEnumerator LerpAngle_Coroutine(Transform doorTransform, float finalValue)
    {
        float timer = 0;
        float duration = 0.25f;
        Quaternion startQuaternion = doorTransform.localRotation;
        Quaternion finalQuaternion = Quaternion.Euler(doorTransform.localRotation.eulerAngles.x, doorTransform.localRotation.eulerAngles.y + finalValue, doorTransform.localRotation.eulerAngles.z);
        while (timer < duration)
        {
            doorTransform.localRotation = Quaternion.Lerp(startQuaternion, finalQuaternion, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        doorTransform.localRotation = finalQuaternion;
    }
}
