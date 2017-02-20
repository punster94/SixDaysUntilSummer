using UnityEngine;
using System.Collections;
using HTC.UnityPlugin.Vive;


public class MiniGameObjectScript : MonoBehaviour
{
    public GameObject controller;

    private bool isAttached = false;

    private int objectType;

    private float timeSinceLastUpdate;
    private float currentDistanceFromController;

    private Vector3 previousPosition;

    void Start()
    {
        previousPosition = transform.position;
        timeSinceLastUpdate = 0;
    }

    void Update()
    {
        if (RaycastScript.minigameObject == gameObject && !isAttached && ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
        {
            attachToController();
        }
        else if (isAttached && ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
        {
            detachFromController();
        }

        if (isAttached)
        {
            updatePositionWhenAttached();
        }

        if (timeSinceLastUpdate >= 0.1f)
        {
            previousPosition = transform.position;
            timeSinceLastUpdate = 0;
        }

        timeSinceLastUpdate += Time.deltaTime;
    }

    public void setObjectData(int type, GameObject ballBucket, GameObject controllerObject)
    {
        controller = controllerObject;
        gameObject.transform.parent = ballBucket.transform;
        objectType = type;
    }

    public int getType()
    {
        return objectType;
    }

    private void attachToController()
    {
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        isAttached = true;
        currentDistanceFromController = Vector3.Distance(transform.position, controller.transform.position);
    }

    private void detachFromController()
    {       
        gameObject.GetComponent<Rigidbody>().AddForce((transform.position - previousPosition) * 1000.0f);
        isAttached = false;
    }

    private void updatePositionWhenAttached()
    {
        currentDistanceFromController = Mathf.Lerp(currentDistanceFromController, 0, 0.25f);
        transform.position = controller.transform.position + (controller.transform.forward * (currentDistanceFromController + 1.0f));
    }
}
