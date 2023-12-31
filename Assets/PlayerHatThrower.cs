using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHatThrower : MonoBehaviour
{

    [SerializeField] private GameObject m_HatPrefab;
    public GameObject m_HatThrowPoint, m_HatHeadPoint, m_HatEndPoint;
    public HatController m_HatController;
    [SerializeField] float m_ThrowForce = 1000f, m_ThrowSpeed = 1000f, m_ThrowBackSpeed = 1000f, m_ThrowDistance = 10f;
    public bool m_IsHatThrown = false;
    // Start is called before the first frame update
    void Start()
    {
        m_HatController = null;

        //if(m_HatController != null)
        //    m_HatController.SetThrowValues(m_ThrowForce, m_ThrowSpeed, m_ThrowBackSpeed, m_ThrowDistance);
    }

    public void ThrowHat()
    {
        if(m_IsHatThrown || m_HatController != null)
            return;
        
        //GameObject hat = Instantiate(m_HatPrefab, transform.position, transform.rotation);
        GameObject hat = GameObject.Instantiate(m_HatPrefab, m_HatThrowPoint.transform.position, m_HatThrowPoint.transform.rotation);
        m_HatController = hat.GetComponent<HatController>();

        m_HatController.SetThrowValues(m_ThrowForce, m_ThrowSpeed, m_ThrowBackSpeed, m_ThrowDistance);
        m_IsHatThrown = true;
        // hat.transform.position = m_HatThrowPoint.transform.position;
        // hat.transform.rotation = m_HatThrowPoint.transform.rotation;
        
        //m_HatController.UnParentHat();
        //m_HatController.LerpHat();
        
        //hat.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
    }

    public void ResetHat()
    {
        m_IsHatThrown = false;
        m_HatController = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
