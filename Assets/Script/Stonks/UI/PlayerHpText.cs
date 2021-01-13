using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHpText : MonoBehaviour {

    void Update() {

        GetComponent<TMPro.TextMeshProUGUI>().text = 
            string.Format("Vie: {0}/{1}", Player.player.hp, Player.player.hpMax);        
    }
}
