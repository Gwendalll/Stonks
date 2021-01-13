using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpriteColorWhenPlayerInvicible : MonoBehaviour {

    public Color color = Color.red;
    
    void Update() {

        Color spriteColor = Color.white;

        if (Player.player.IsInvincible()) {
            spriteColor = color;
        }

        GetComponent<SpriteRenderer>().color = spriteColor;
    }
}
