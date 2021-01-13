using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using static UnityEditor.EditorGUILayout;
#endif

namespace Sequencer {

    public enum ScrollDirection {
        RIGHT,
        LEFT,
        UP,
        DOWN,
    }
    
    public class Sequencer : MonoBehaviour {

        public ScrollDirection direction = ScrollDirection.UP;
        public float triggerSize = 30f;
        public float velocity = 2f;
        public float timeScale = 1f;

        public Color gizmosColor = new Color(1f, 0.5f, 1f);
        public bool hidePassedTriggers = true;

        public float scroll = 0;

        public Vector3 GetSequencerPosition(Vector3 position) => transform.InverseTransformPoint(position);
        public Vector3 GetScrollPosition(Vector3 position) => scroller.transform.InverseTransformPoint(position);

        void Start() {
            Prepare();
        }

        public Vector3 GetScrollDirection() {
            switch (direction) {
                case ScrollDirection.RIGHT:
                return Vector3.left;
                
                case ScrollDirection.LEFT:
                return Vector3.right;
                
                case ScrollDirection.DOWN:
                return Vector3.up;
                
                case ScrollDirection.UP:
                default:
                return Vector3.down;
            }
        }

        void UpdateScrollerPosition() {
            scroller.transform.localPosition = GetScrollDirection() * scroll;
        }

        void Update() {
            scroll += velocity * timeScale * Time.deltaTime * Item.timeScale;
            UpdateScrollerPosition();
        }

        public Trigger GetTriggerByName(string name) =>
            GetComponentsInChildren<Trigger>().FirstOrDefault(x => x.gameObject.name == name);

        public void Jump(float destination) {
            scroll = destination;
            UpdateScrollerPosition();
            foreach(var sequence in GetComponentsInChildren<Trigger>()) {
                sequence.ResetTrigger();
            }
        }
        public void Jump(Trigger trigger) {
            switch(direction) {
                case ScrollDirection.LEFT:
                case ScrollDirection.RIGHT:
                Jump(GetScrollPosition(trigger.transform.position).x);
                break;
                case ScrollDirection.DOWN:
                case ScrollDirection.UP:
                Jump(GetScrollPosition(trigger.transform.position).y);
                break;
            }
        }
        public void Jump(string name) {

            var sequenceTrigger = GetTriggerByName(name);

            if (sequenceTrigger != null) {

                Jump(sequenceTrigger);

            } else {

                Debug.LogFormat("Il n'y a pas d'objet <SequenceTrigger> qui s'appelle \"{0}\".", name);
            }
        }

        void DrawArrow(Vector3 position, float size) {
            Gizmos.DrawLine(position, position + new Vector3(size, size, 0f));
            Gizmos.DrawLine(position, position + new Vector3(size, -size, 0f));
        }

        void OnDrawGizmos() {
            Gizmos.color = gizmosColor;

            switch(direction) {
                case ScrollDirection.RIGHT:
                Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, 0f));
                break;
                case ScrollDirection.LEFT:
                Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, 180f));
                break;
                case ScrollDirection.UP:
                Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, 90f));
                break;
                case ScrollDirection.DOWN:
                Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, -90f));
                break;
            }
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(0f, triggerSize + 2f, 2f));
            
            int max = Mathf.FloorToInt(triggerSize / 2f / 2f);
            for (int i = -max; i <= max; i++) {
                DrawArrow(new Vector3(0f, i * 2f, 0f), 0.5f);
            }

            Gizmos.color = new Color(gizmosColor.r, gizmosColor.g, gizmosColor.b, 0.25f);
            Gizmos.DrawCube(Vector3.zero, new Vector3(0f, triggerSize + 2f, 2f));
        }

        Transform scroller;
        void Prepare() {
            if (scroller == null) {
                scroller = transform.Find("Scroller");
                if (scroller == null) {
                    var go = new GameObject("Scroller");
                    scroller = go.transform;
                    scroller.SetParent(transform);
                }
            }
            scroller.transform.localRotation = Quaternion.identity;
            foreach(var sequence in GetComponentsInChildren<Sequence>()) {
                sequence.transform.SetParent(scroller, false);
            }
            UpdateScrollerPosition();
        }

        public void CreateNewSequence() {
            Prepare();
            
            var last = GetComponentsInChildren<Sequence>().LastOrDefault();

            var go = new GameObject("Sequence");
            go.transform.SetParent(scroller.transform, false);
            go.AddComponent<Sequence>();

            if (last != null) {
                go.transform.position = last.transform.position + GetScrollDirection() * -last.sequenceLength;
            }
        }

        [SerializeField]
        private float AlignSequences_Spacing = 0f;
        void AlignSequences() {
            float x = 0f;
            foreach(var sequence in GetComponentsInChildren<Sequence>()) {
                sequence.transform.localPosition = GetScrollDirection() * -x;
                sequence.transform.localRotation = Quaternion.identity;
                x += sequence.sequenceLength + AlignSequences_Spacing;
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Sequencer))]
        class MyEditor : Editor {
            Sequencer sequencer => target as Sequencer;
            public override void OnInspectorGUI() {
                PropertyField(serializedObject.FindProperty("direction"));
                LabelField("Scroll", EditorStyles.boldLabel);
                var scrollProp = serializedObject.FindProperty("scroll");
                PropertyField(scrollProp);
                if (serializedObject.hasModifiedProperties) {
                    scrollProp.floatValue = Mathf.Max(scrollProp.floatValue, 0f);
                    serializedObject.ApplyModifiedProperties();
                    sequencer.Prepare();
                }

                Space(16);
                LabelField("Properties", EditorStyles.boldLabel);
                PropertyField(serializedObject.FindProperty("triggerSize"));
                PropertyField(serializedObject.FindProperty("velocity"));
                PropertyField(serializedObject.FindProperty("timeScale"));

                Space(16);
                LabelField("Gizmos", EditorStyles.boldLabel);
                PropertyField(serializedObject.FindProperty("gizmosColor"));
                PropertyField(serializedObject.FindProperty("hidePassedTriggers"));

                Space(16);
                LabelField("Sequences", EditorStyles.boldLabel);
                if (GUILayout.Button("Create New Sequence")) {
                    sequencer.CreateNewSequence();
                }
                
                BeginHorizontal();
                sequencer.AlignSequences_Spacing = EditorGUILayout.FloatField("Spacing", sequencer.AlignSequences_Spacing);
                if (GUILayout.Button("Align Sequences")) {
                    sequencer.AlignSequences();
                }
                EndHorizontal();

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
