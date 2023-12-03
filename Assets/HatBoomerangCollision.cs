using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatBoomerangCollision : MonoBehaviour
{
    [SerializeField] PlayerHatThrower m_HatThrower;
    HatController m_HatController;

    private void Start()
    {
        m_HatThrower = FindObjectOfType<PlayerHatThrower>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerAttack") && other.gameObject.GetComponent<HatController>().throwing == false)
        {
            m_HatController = other.gameObject.GetComponent<HatController>();
            m_HatController.DestroyHat();
            
            m_HatThrower.ResetHat();
        }        
    }
}
