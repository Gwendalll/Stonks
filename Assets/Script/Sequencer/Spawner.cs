using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using static UnityEditor.EditorGUILayout;
#endif

namespace Sequencer {

    public class SequenceSpawnerLink : MonoBehaviour {
        public Spawner spawner;
    }

    [ExecuteAlways]
    public class Spawner : Trigger {

        public Item prefab;

        public int spawnCount = 0;

        protected override string GetName() => string.Format("Spawner:{0}", prefab !=null ? prefab.gameObject.name : "None");
        protected override string GetHandleLabel() => prefab !=null ? prefab.gameObject.name : "None";

        protected override void DoTrigger() {

            if (prefab != null) {
                var spawned = Instantiate(prefab, GetTriggerPosition(), Quaternion.identity);
                var link = spawned.gameObject.AddComponent<SequenceSpawnerLink>();
                link.spawner = this;

                foreach (var item in spawned.GetComponentsInChildren<Item>()) {
                    item.Step(Time.fixedDeltaTime * triggerDelta);
                }

                spawnCount++;
            }
        }

#if UNITY_EDITOR

        void OnDrawGizmos() {
            DrawGizmos();
        }

        [CustomEditor(typeof(Spawner))]
        class MyEditor : Editor {
            Spawner Target => target as Spawner;
            public override void OnInspectorGUI() {


                LabelField("Prefab", EditorStyles.boldLabel);
                PropertyField(serializedObject.FindProperty("prefab"));

                Space(16);
                LabelField("Trigger Properties", EditorStyles.boldLabel);
                DrawPropertiesExcluding(serializedObject, "prefab");
                serializedObject.ApplyModifiedProperties();

                bool show = Target.transform.childCount == 0;
                string label = show ? "Show Prefab" : "Hide Prefab";
                if (show) {
                    var enabled = GUI.enabled;
                    GUI.enabled = Target.prefab != null;
                    if (GUILayout.Button(label)) {
                        var preview = Instantiate(Target.prefab, Target.transform.position, Quaternion.identity, Target.transform);
                        preview.gameObject.hideFlags = HideFlags.HideInHierarchy;
                    }
                    GUI.enabled = enabled;
                } else {
                    if (GUILayout.Button(label)) {
                        DestroyImmediate(Target.transform.GetChild(0).gameObject);
                    }
                }

                LabelField(string.Format("Sequencer found: {0}", Target.sequencer));
            }
        }
#endif
    }
}
