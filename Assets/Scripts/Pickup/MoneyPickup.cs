using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    [SerializeField] AudioClip moneyPickup;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if( collision.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(moneyPickup, Camera.main.transform.position);
            Destroy(gameObject);
        }
    }
}
