using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BramblesScript : MonoBehaviour
{
    public float damageAmount = 10f; 
    public float knockbackForce = 5;

    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("Hit player");
            Vector2 knockAngle = (collision.transform.position - this.transform.position).normalized * knockbackForce;
            DamageInfo brambleDamage = new DamageInfo(damageAmount, knockAngle, AuraType.Normal);
            collision.gameObject.GetComponent<Health>().Damage(brambleDamage);
            collision.gameObject.GetComponent<Rigidbody>().AddForce(knockAngle, ForceMode.Impulse);
        }
    }
}
