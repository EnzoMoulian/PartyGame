using UnityEngine;

public class HealthBarBehavior : MonoBehaviour
{
    private float f_previousHealth;

    void Start()
    {
        f_previousHealth = GetComponentInParent<PlayerInfo>().playersCurrentHealth;
    }

    void Update()
    {
        if (f_previousHealth != GetComponentInParent<PlayerInfo>().playersCurrentHealth && GetComponentInParent<PlayerInfo>().playersCurrentHealth >= 0)
        {
            transform.localScale = new Vector3(GetComponentInParent<PlayerInfo>().playersCurrentHealth / GameManager._instance.f_initialHealth, transform.localScale.y, transform.localScale.z);
            f_previousHealth = GetComponentInParent<PlayerInfo>().playersCurrentHealth;
        }
    }
}
