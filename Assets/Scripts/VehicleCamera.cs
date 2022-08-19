using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class VehicleCamera : MonoBehaviour
{
    public static VehicleCamera Instance;

    [SerializeField] private Vehicle vehicle;
    [SerializeField] private Vector3 offset;

    [SerializeField] private float rotateSensetive;
    [SerializeField] private float scrollSensetive;
    
    [SerializeField] private float maxVerticalAngle;
    [SerializeField] private float minVerticalAngle;

    [SerializeField] private float distance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float minDistance;
    [SerializeField] private float distanceOffsetFromCollisionHit;
    [SerializeField] private float distanceLerpRate;

    [SerializeField] private GameObject zoomMaskEffect;    
    [SerializeField] private float zoomedFov;
    [SerializeField] private float zoomedMaxVerticalAngle;

    private new Camera camera;

    private Vector2 rotationControl;

    private float defaultFov;
    private float deltaRotationX;
    private float deltaRotationY;
    private float currentDistance;
    private float defaultMaxVerticalAngle;
    private float lastDistance;

    private bool isZoom;
    public bool IsZoom => isZoom;


    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        camera = GetComponent<Camera>();
        defaultFov = camera.fieldOfView;
        defaultMaxVerticalAngle = maxVerticalAngle;
    }
    private void Update()
    {
        if (vehicle == null) return;

        UpdateControle();

        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        isZoom = distance <= minDistance;

        deltaRotationX += rotationControl.x * rotateSensetive;
        deltaRotationY += rotationControl.y * -rotateSensetive;

        deltaRotationY = ClampAngle(deltaRotationY, minVerticalAngle, maxVerticalAngle);

        Quaternion finalRotation = Quaternion.Euler(deltaRotationY, deltaRotationX, 0);
        Vector3 finalPosition = vehicle.transform.position - (finalRotation * Vector3.forward * distance);
        finalPosition = AddLocalOffset(finalPosition);

        float targetDistance = distance;
        RaycastHit hit;

        if(Physics.Linecast(vehicle.transform.position + new Vector3(0, offset.y,0), finalPosition, out hit) == true)
        {
            float distanceToHit = Vector3.Distance(vehicle.transform.position + new Vector3(0, offset.y, 0), hit.point);

            if(hit.transform != vehicle)
            {
                if (distanceToHit < distance)
                targetDistance = distanceToHit - distanceOffsetFromCollisionHit;
            }              
        }

        currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, Time.deltaTime * distanceLerpRate);
        currentDistance = Mathf.Clamp(currentDistance, minDistance, distance);

        finalPosition = vehicle.transform.position - (finalRotation * Vector3.forward * currentDistance);

        transform.rotation = finalRotation;
        transform.position = finalPosition;
        transform.position = AddLocalOffset(transform.position);

        zoomMaskEffect.SetActive(isZoom);
        if(isZoom == true)
        {
            transform.position = vehicle.ZoomOpticsPosition.position;
            camera.fieldOfView = zoomedFov;
            maxVerticalAngle = zoomedMaxVerticalAngle;
        }
        else
        {
            camera.fieldOfView = defaultFov;
            maxVerticalAngle = defaultMaxVerticalAngle;
        }
    }

    private void UpdateControle()
    {
        rotationControl = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        distance += -Input.mouseScrollDelta.y * scrollSensetive;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isZoom = !isZoom;

            if(isZoom == true)
            {
                lastDistance = distance;
                distance = minDistance;
            }
            else
            {
                distance = lastDistance;
                currentDistance = lastDistance;
            }
        }
    }

    private Vector3 AddLocalOffset(Vector3 position)
    {
        Vector3 result = position;
        result += new Vector3(0, offset.y, 0);
        result += transform.right * offset.x;
        result += transform.forward * offset.z;
        return result;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if(angle <  -360)
        {
            angle += 360;
        }

        if(angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }

    public void SetTarget(Vehicle target)
    {
        vehicle = target;
    }
}
