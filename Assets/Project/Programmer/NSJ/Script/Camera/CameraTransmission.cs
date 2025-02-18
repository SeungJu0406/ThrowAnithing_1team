using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransmission : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;

    Camera _cam;
    float distance;
    RaycastHit[] hits =new RaycastHit[20];
    int hitCount;

    private void Start()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);

        StartCoroutine(TransmitRoutine());
    }

    IEnumerator TransmitRoutine()
    {
        while (true) 
        {
            Vector3 cameraDir = Camera.main.transform.position - (transform.position + _offset);

            // 기존 hits 레이어 복구
            for (int i = 0; i < hitCount; i++)
            {
                if (hits[i].transform.gameObject == null)
                    continue;

                hits[i].transform.gameObject.layer = Layer.Wall;
            }

            // 새로운 hits 레이어 세팅
            hitCount = Physics.RaycastNonAlloc(transform.position, cameraDir.normalized, hits, distance, 1 << Layer.Wall);
            for (int i = 0; i < hitCount; i++)
            {
                hits[i].transform.gameObject.layer = Layer.HideWall;
            }
            yield return 0.1f.GetDelay();
        }
    }
}
