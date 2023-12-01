using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerCombat : MonoBehaviour
{
    PlayerAnimationManager _playerAnimationManager;
    
    [SerializeField]
    private GameObject visualEffect;
    public PlayerHatThrower _playerHatThrower;

    private float _canAttack = -1f;
    private float _attackRate = 1.5f;
    private string ATTACK_ANIM = "Attack";

    private void Start()
    {
        _playerAnimationManager = GetComponent<PlayerAnimationManager>();
    }
    public void BasicAttack()
    {
        if (Time.time > _canAttack)
        {
            _playerAnimationManager.ChangeAnimationState(ATTACK_ANIM);
            _canAttack = Time.time + _attackRate;
            visualEffect.SetActive(true);
            _playerHatThrower.ThrowHat();
            StartCoroutine(TurnOffVFX());
        }
    }
    IEnumerator TurnOffVFX() //probably wont keep(placeholder)
    {
        yield return new WaitForSeconds(1);
        visualEffect.SetActive(false);
    }

}
