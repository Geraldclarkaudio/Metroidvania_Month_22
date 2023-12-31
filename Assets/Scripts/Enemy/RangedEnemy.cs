using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : EnemyBase
{
    [SerializeField]
    private GameObject projectile;
    private float _canFire = -1f;
    private float fireRate = 1.0f;
    [SerializeField]
    private bool _followsPlayer;
    public override void Start()
    {
        base.Start();
        enemyState = EnemyState.attacking;
    }
    public override void Attack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if(_followsPlayer == true)
        {
            transform.LookAt(_player.transform.position);

            if (distanceToPlayer < 15 && Time.time > _canFire)
            {
                Fire();
            }
        }

        else
        {
            //charge up animation or something?
            if(Time.time > _canFire)
            {
                Fire();
            }
        }
    }

    void Fire()
    {
        Instantiate(projectile, transform.position, transform.rotation);
        _canFire = Time.time + fireRate;
    }

    public override void TakeDamage(DamageDealer damageDealer)
    {
        _health -= damageDealer.damageAmount.Value;
        if (_health <= 0)
        {
            Death();
        }
    }
    public override void Death()
    {
        Destroy(this.gameObject);
    }
}
