using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Interactable : MonoBehaviour
{
    public GameManager gameManager;

    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }
    
    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public abstract void Interact();

    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if(collision.CompareTag("player"))
            //collision.GetComponent<PlayerScript>().DoSomething()
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if(collision.CompareTag("player"))
            //collision.GetComponent<PlayerScript>().DoSomeOtherThing()
    }
    */
}
