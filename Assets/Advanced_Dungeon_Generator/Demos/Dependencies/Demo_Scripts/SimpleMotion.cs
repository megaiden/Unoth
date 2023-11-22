using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMotion : MonoBehaviour
{
    public float deltaY;
    public float deltaZ;
    public float deltaMotion;
    public float deltaRotateSpeed;

    private void OnTriggerExit(Collider collider)
    {
        Charge(collider);
    }

    public void Charge(Collider collider)
    {
        Vector3 playerDirection = collider.transform.position - transform.position;
        playerDirection.Normalize();

        float razon = Vector3.Dot(transform.forward, playerDirection);

        if (razon < 0)
        {
            Vector3 centre = transform.parent.transform.position;
            float start = collider.gameObject.transform.position.x;
            float end = ((centre.x - start) * 2) + start - 1;
            Vector3 destination = new Vector3(end, deltaY * 9, centre.z + deltaMotion);

            centre.z += deltaZ - 0.5f;
            centre.x -= 0.5f;
            collider.transform.parent.gameObject.GetComponent<SimplePlayerMovement>().Motion(centre, deltaY, deltaRotateSpeed, destination);
        }
    }
}
