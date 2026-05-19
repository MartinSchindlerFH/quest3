using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform respawnPoint;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var character = other.GetComponentInChildren<Character>();
            character.SetHealth(0);
        }
        /*
        var controller = other.gameObject.GetComponent<CharacterController>();
        controller.enabled = false;
        controller.transform.position = this.respawnPoint.transform.position;
        controller.enabled = true;
        */
    }
}
