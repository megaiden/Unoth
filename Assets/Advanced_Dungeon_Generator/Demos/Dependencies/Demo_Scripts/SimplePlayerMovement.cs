using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    private Vector3 buffer;
    private Vector3 placeholder;
    private Vector3 rawDirection;
    private Vector3 destination;
    private Vector3 stepDestination;

    private RaycastHit hit;
    private float nextActionTime;
    private bool isMoving = false;
    private bool isRotating = false;
    private Vector3 centre;
    private float rotateSpeed;
    private float deltaY;

    private bool playerCanMove = false;

    public LayerMask walkableLayer;
    public float playerHeight = 2.0f;
    public float playerSpeed = 2.0f;
    public float timePeriod = 0.015f;



    private void Start()
    {
        DungeonGenerator.Instance.OnDungeonComplete += SpawnPlayer;
    }


    void Update()
    {
        if (!playerCanMove)
            return;


        if (!isRotating)
        {
            rawDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            if (Time.time > nextActionTime && !isMoving)
            {
                nextActionTime = Time.time + timePeriod;

                if (rawDirection.x != 0 || rawDirection.z != 0)
                {
                    if (Physics.Raycast(transform.position + rawDirection, Vector3.down, out hit, 5.0f, walkableLayer.value))
                    {
                        destination = transform.position + rawDirection;
                        destination.y += (playerHeight / 2) - hit.distance;
                        isMoving = true;
                        buffer = rawDirection;
                    }
                }
            }

            if (isMoving)
            {
                float step = playerSpeed * Time.deltaTime;
                placeholder = Vector3.MoveTowards(transform.position, destination, step);

                if (placeholder == destination)
                {
                    if (rawDirection == buffer && Physics.Raycast(transform.position + rawDirection, Vector3.down, out hit, 5.0f, walkableLayer.value))
                    {
                        destination += buffer;
                        destination.y += (playerHeight / 2) - hit.distance;
                        placeholder = Vector3.MoveTowards(transform.position, destination, step);
                        transform.position = placeholder;
                    }
                    else
                    {
                        transform.position = destination;
                        isMoving = false;
                    }
                }
                else
                    transform.position = placeholder;
            }
        }
        else
        {
            transform.RotateAround(centre, Vector3.up, rotateSpeed * Time.deltaTime);
            transform.position += new Vector3(0, deltaY * Time.deltaTime, 0);
            
            if(Vector3.Distance(transform.position, stepDestination) <= 0.3)
            {
                transform.position = stepDestination;
                isRotating = false;
                isMoving = false;
            }
        }
    }

    public void SpawnPlayer()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        playerCanMove = true;

        //Debug.Log("Im ready!");
    }

    private void OnDisable()
    {
        DungeonGenerator.Instance.OnDungeonComplete -= SpawnPlayer;
    }


    public void Motion(Vector3 centre, float deltaY, float rotateSpeed, Vector3 stepDestination)
    {
        this.centre = centre;
        this.deltaY = deltaY;
        this.rotateSpeed = rotateSpeed;
        stepDestination.y += transform.position.y;
        this.stepDestination = stepDestination;
        isRotating = true;
    }
}
