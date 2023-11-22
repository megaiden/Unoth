using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    public Vector3 offset;

    private Transform viewCamera;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        viewCamera = gameObject.transform;
        offset = viewCamera.position - target.position;
    }

    void Update()
    {
        Vector3 targetPosition = target.position + offset;
        viewCamera.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
