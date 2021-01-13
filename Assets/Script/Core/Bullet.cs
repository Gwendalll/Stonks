using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 1f;

    private void OnTriggerEnter(Collider other) {

        Item otherItem = 
            // Tente de récupérer une instance de <Item> directement sur le gameObject associé à "other"
            other.GetComponent<Item>() 
            // ou, si null, tente de récupérer une instande de <Item> parmi les parents de "other"
            ?? other.GetComponentInParent<Item>();
        
        if (otherItem != null) {
            otherItem.SetDamage(damage);
        }

        Destroy(gameObject);
        GetComponent<Explosion>()?.Explode();
    }
}
