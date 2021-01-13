using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum BossArmType {
    Left,
    Right,
}

public class BossArm : MonoBehaviour
{
    public BossArmType type = BossArmType.Left;

    public void Attack() {
        GetComponentInChildren<Animator>().SetTrigger("Attack");
    }

    [CustomEditor(typeof(Editor))]
    class MyEditor : Editor {
        BossArm bossArm => target as BossArm;
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (GUILayout.Button("Attack")) {
                bossArm.Attack();
            }
        }
    }
}
