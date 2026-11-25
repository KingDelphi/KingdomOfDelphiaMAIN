using System.Collections;
using UnityEngine;


public class CinemachineZoomController : MonoBehaviour
{
    private Unity.Cinemachine.CinemachineVirtualCamera virtualCam;
    private float originalSize;
    private Vector3 originalPosition;

    [SerializeField] private float zoomFactor = 0.5f;
    [SerializeField] private float zoomDuration = 0.5f;

    private void Start()
    {
        virtualCam = GetComponent<Unity.Cinemachine.CinemachineVirtualCamera>();
        if (virtualCam != null)
        {
            originalSize = virtualCam.m_Lens.OrthographicSize;
            originalPosition = virtualCam.transform.position;
        }
    }

    public void ZoomToTarget(Transform target)
    {
        if (virtualCam != null)
        {
            StartCoroutine(ZoomCoroutine(target));
        }
    }

    private IEnumerator ZoomCoroutine(Transform target)
    {
        float targetSize = originalSize * zoomFactor;
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, virtualCam.transform.position.z);

        float elapsed = 0;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            virtualCam.m_Lens.OrthographicSize = Mathf.Lerp(originalSize, targetSize, elapsed / zoomDuration);
            virtualCam.transform.position = Vector3.Lerp(originalPosition, targetPos, elapsed / zoomDuration);
            yield return null;
        }
    }
}
