using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sequencer;

[RequireComponent(typeof(Sequencer.Trigger))]
public class Jump : MonoBehaviour {

    [Header("Choose a target — or a targetName.")]
    public Sequencer.Trigger target;
    public string targetName;

    [Tooltip("Nombre de saut max.\nSi valeur négative (-1) -> Boucle infinie.")]
    public int jumpMaxCount = -1;

    void OnTriggerSequence(Sequencer.Trigger trigger) {
        
        if (target != null) {
            trigger.sequencer.Jump(target);
        }
        else if (targetName != null) {
            trigger.sequencer.Jump(targetName);
        }

        jumpMaxCount--;

        if (jumpMaxCount == 0) {
            gameObject.SetActive(false);
        }
    }
}
