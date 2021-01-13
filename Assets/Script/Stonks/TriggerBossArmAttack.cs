using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBossArmAttack : MonoBehaviour {
    
    public BossArmType type = BossArmType.Left;

    void OnTriggerSequence(Sequencer.Trigger trigger) {
        
        foreach(var arm in GameObject.FindObjectsOfType<BossArm>()) {

            if (arm.type == type) {
                arm.Attack();
            }
        }
    }
}
