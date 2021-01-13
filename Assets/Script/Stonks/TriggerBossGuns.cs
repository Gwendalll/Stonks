using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBossGuns : MonoBehaviour {

    public bool gunActive = true;

    void OnTriggerSequence(Sequencer.Trigger trigger) {

        // NOTE: le "?" (discret) sert ici à faire un test : 
        // Si le composant <BossGuns> est trouvé dans la scène,
        // alors appeler la méthode "SetGunsActive"
        FindObjectOfType<BossGuns>()?.SetGunsActive(gunActive);
    }
}
