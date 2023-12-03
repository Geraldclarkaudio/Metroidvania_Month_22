using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatBoomerangCollision : MonoBehaviour
{

    HatController m_HatController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerAttack") && 
            other.gameObject.GetComponent<HatController>())
        {
            m_HatController = other.gameObject.GetComponent<HatController>();
            other.gameObject.GetComponent<HatController>().ParentHat();
        }
        // else if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<Enemy>().isAlive)
        // {
        //     other.gameObject.GetComponent<Enemy>().TakeDamage(1);
        // }
    }
}
