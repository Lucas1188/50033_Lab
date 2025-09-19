using System;
using UnityEngine;

public class Goomba : EnemyBase, IDamage, IHealth
{
    [SerializeField]
    private float damage;
    [SerializeField]
    private float health;
    private float currentHealth;
    private float currentDamage;
    public void Buff(float value, DamageBuffOperator damageBuffOperator, TimeSpan timeSpan)
    {
        var o_damage = currentDamage;
        switch (damageBuffOperator)
        {
            case DamageBuffOperator.Add:
                currentDamage += value;
                break;
            case DamageBuffOperator.Multiply:
                currentDamage *= value;
                break;
        }
        if (currentDamage > o_damage || currentDamage <= 0)
        {
            currentDamage = o_damage;
        }
    }

    public float DealDamage(DamageModificationAttributes[] damageModificationAttributes)
    {
        return currentDamage;
    }

    public void DeBuff(float value, DamageBuffOperator damageBuffOperator, TimeSpan timeSpan)
    {
        var o_damage = currentDamage;
        switch (damageBuffOperator)
        {
            case DamageBuffOperator.Add:
                currentDamage += value;
                break;
            case DamageBuffOperator.Multiply:
                currentDamage *= value;
                break;
        }
        if (currentDamage > o_damage || currentDamage <= 0)
        {
            currentDamage = o_damage;
        }
    }

    public void TakeDamage(float damage)
    {
        if (_currentState != EnemyState.Play) return;
        if (damage >= 0)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                this.Die();
            }
        }
    }

    public void TakeHealth(float health)
    {
        if (_currentState != EnemyState.Play) return;
        if (health >= 0)
        {
            currentHealth += health;
            if (currentHealth > health)
            {
                currentHealth = health;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        base.Start();
        currentHealth = health;
        currentDamage = damage;
    }
    protected void FixedUpdate()
    {
        base.FixedUpdate();
        //Debug.Log(this.rigidbody2D.transform.position);
    }

#nullable enable

    void OnCollisionEnter2D(Collision2D coll)
    {
        var _dir = this.rigidbody2D.transform.position - coll.transform.position;
        if (coll.otherCollider.gameObject.CompareTag("Friendly") && _currentState == EnemyState.Play)
        {
            ContactPoint2D[] contacts = new ContactPoint2D[3];

            //Die here if the collision came from above
            var c_count = coll.GetContacts(contacts);
            for (int i = 0; i < c_count; i++)
            {
                Debug.Log(contacts[i].ToString());//Should we directly get the componenet here or go through another manager so we can track stuff?
                                                  //Test only to change later on
            }
            var dmg_modifiers = coll.otherCollider.gameObject.GetComponent<IDamage>().GetDamageModificationAttributes();
            coll.otherCollider.gameObject.GetComponent<IHealth>().TakeDamage(DealDamage(dmg_modifiers));
        }
        else if(!coll.otherCollider.gameObject.CompareTag("Ground"))
        {
            
            this.dir = _dir.x >= 0 ? 1 : -1;
            Debug.Log($"Goomba Hit something {_dir} {this.rigidbody2D.transform.position} {coll.otherCollider.transform.position}setting dir to {this.dir}");
            this.rigidbody2D.linearVelocityX = this.dir * this.Speed;
        }
    }
    public DamageModificationAttributes[]? GetDamageModificationAttributes()
    {
        return null;
    }
#nullable disable
}
