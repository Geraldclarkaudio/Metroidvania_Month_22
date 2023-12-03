using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HatController : MonoBehaviour
{
    [SerializeField] private PlayerHatThrower m_HatThrower;
    [SerializeField] float m_ThrowForce = 1000f, m_ThrowSpeed = 5f, m_ThrowBackSpeed = 1000f, m_ThrowDistance = 10f;

    public Transform m_EndPosition;
    public bool throwing = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetThrowValues(float throwForce, float throwSpeed, float throwBackSpeed, float throwDistance)
    {
        m_ThrowForce = throwForce;
        m_ThrowSpeed = throwSpeed;
        m_ThrowBackSpeed = throwBackSpeed;
        m_ThrowDistance = throwDistance;
        m_EndPosition = m_HatThrower.m_HatEndPoint.transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_HatThrower.m_IsHatThrown)
        {
            /* transform.position = m_HatThrower.transform.position;
            transform.rotation = m_HatThrower.transform.rotation; */
           // UnParentHat();

            if(transform.position == m_HatThrower.m_HatThrowPoint.transform.position)
            {
                throwing = true;
            }
            else if(transform.position == m_EndPosition.position)
            {
                throwing = false;
            }

            if(throwing == true)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_EndPosition.position, m_ThrowSpeed * Time.deltaTime);
            }
            else if(throwing == false)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, m_HatThrower.m_HatThrowPoint.transform.position, 
                    m_ThrowSpeed * Time.deltaTime * (transform.position - m_HatThrower.m_HatThrowPoint.transform.position).magnitude);
                if(transform.position == m_HatThrower.m_HatThrowPoint.transform.position)
                {
                    m_HatThrower.m_IsHatThrown = false;
                    ParentHat();
                }
            }
        }
    }

    //Lerp thrown hat across Throw Distance and return to player when it reaches the end
    public void LerpHat()
    {
        StartCoroutine(ThrowForwardCoroutine());
    }

    public void LerpHatBack()
    {
        StartCoroutine(ThrowReturnCoroutine());
    }

    public void UnParentHat()
    {
        transform.parent = null;
    }

    public void ParentHat()
    {
        throwing = false;
        transform.parent = m_HatThrower.transform;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = m_HatThrower.m_HatHeadPoint.transform.position;
    }

    IEnumerator ThrowForwardCoroutine()
    {
        float t = 0;
        Vector3 startPosition = m_HatThrower.m_HatThrowPoint.transform.position;
        Vector3 endPosition = m_HatThrower.transform.position + new Vector3(0, 0, m_ThrowDistance);
        while (t < 1)
        {
            t += Time.deltaTime * m_ThrowSpeed;
            transform.position = Vector3.Lerp(startPosition, m_EndPosition.position, t);
            yield return null;
        }
        m_HatThrower.m_IsHatThrown = true;
    }

    IEnumerator ThrowReturnCoroutine()
    {
        float t = 0;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = m_HatThrower.transform.position;
        while (t < 1)
        {
            t += Time.deltaTime * m_ThrowSpeed;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
        m_HatThrower.m_IsHatThrown = false;
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Enemy")
        {
            throwing = false;            
            //LerpHatBack();
        }
        else if(other.gameObject.tag == "PlayerDamageBox" || other.gameObject.tag == "Player" || other.gameObject.tag == "PlayerHands")
        {            
            ParentHat();
        }
    }
}
