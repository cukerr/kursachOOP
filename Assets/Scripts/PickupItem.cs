using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum PickupType { Speed, Health, Damage }
    public PickupType type;
    public float duration = 10f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerScript player = other.GetComponent<PlayerScript>();
            if (player != null)
            {
                player.ApplyPickup(type, duration);
                Destroy(gameObject); // Óםטקעמזאול ןנוהלוע ןמסכו ןמהבמנא
            }
        }
    }
}
