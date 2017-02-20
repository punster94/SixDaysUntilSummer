using UnityEngine;
using System.Collections;

public class CharacterManager : MonoBehaviour
{
    public int characterId;

    public int[] likes;

    public GameStateManager gameStateManager;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<MiniGameObjectScript>() != null)
        {
            addLpFromCollision(collision.gameObject.GetComponent<MiniGameObjectScript>());
            Destroy(collision.gameObject);
        }
    }

    public void addLpFromCollision(MiniGameObjectScript miniGameObject)
    {
        gameStateManager.addToLoveState(characterId, likes[miniGameObject.getType()]);
    }
}
