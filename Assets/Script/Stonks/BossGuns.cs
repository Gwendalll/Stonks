using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGuns : MonoBehaviour {

    public void SetGunsActive(bool active) {

        transform.Find("Guns").gameObject.SetActive(active);
    }
}
