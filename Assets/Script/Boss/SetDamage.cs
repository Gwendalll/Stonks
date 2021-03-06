using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDamage : MonoBehaviour
{
    public float damage = 1f;

    private void OnTriggerEnter(Collider other) {

        Item otherItem = other.GetComponent<Item>();
        
        if (otherItem != null) {
            otherItem.SetDamage(damage);
        }
    }
}
