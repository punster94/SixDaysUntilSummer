using UnityEngine;
using UnityEngine.UI;

public class RaycastScript : MonoBehaviour
{
    [SerializeField]
    public float interactionDistance = 3f;
    static public bool canInteract;
    static public GameObject interactingObject;

    static public Image imageBeingPointedAt;

    static public GameObject minigameObject;

    void Update()
    {
        imageBeingPointedAt = null;
        canInteract = false;
        minigameObject = null;

        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward * interactionDistance, Color.cyan, Time.deltaTime);

        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance))
        {
            interactingObject = hit.transform.gameObject;

            if (interactingObject.GetComponent<CharacterManager>() != null)
            {
                canInteract = true;
            }

            if (interactingObject.GetComponent<Image>() != null)
            {
                imageBeingPointedAt = interactingObject.GetComponent<Image>();
            }

            if(interactingObject.GetComponent<MiniGameObjectScript>()!=null)
            {
                minigameObject = interactingObject;
            }
        }
    }
}
