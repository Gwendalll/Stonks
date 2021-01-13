using System.Collections;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sequencer {

    public class Sequence : MonoBehaviour {

        public float sequenceLength = 20f;





        static void GizmosAlpha(float value) {
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, value);
        }

#if UNITY_EDITOR
        void DrawRect(Vector3 position, Vector3 size) {
            GizmosAlpha(1f);
            Gizmos.DrawWireCube(position, size);
            if (IsSelected) {
                GizmosAlpha(0.05f);
                Gizmos.DrawCube(position, size);
            }
        }

        void DrawLabel(Vector3 offset) {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Gizmos.color;
            Handles.Label(transform.position + offset, gameObject.name, style);
        }

        void DrawRight(float triggerSize) {
            DrawRect(Vector3.right * sequenceLength / 2f, new Vector3(sequenceLength, triggerSize, 0));
            GizmosAlpha(0.1f);
            int max = Mathf.FloorToInt(triggerSize / 2f);
            for (int i = -max; i <= max; i++) {
                Gizmos.DrawLine(new Vector3(0f, i, 0f), new Vector3(sequenceLength, i, 0f));
            }
            DrawLabel(Vector3.right * sequenceLength / 2f + Vector3.down * (triggerSize / 2f + 1f));
        }

        void DrawLeft(float triggerSize) {
            DrawRect(Vector3.left * sequenceLength / 2f, new Vector3(sequenceLength, triggerSize, 0));
            GizmosAlpha(0.1f);
            int max = Mathf.FloorToInt(triggerSize / 2f);
            for (int i = -max; i <= max; i++) {
                Gizmos.DrawLine(new Vector3(-sequenceLength, i, 0f), new Vector3(0, i, 0f));
            }
            DrawLabel(Vector3.left * sequenceLength / 2f + Vector3.down * (triggerSize / 2f + 1f));
        }

        void DrawUp(float triggerSize) {
            DrawRect(Vector3.up * sequenceLength / 2f, new Vector3(triggerSize, sequenceLength, 0));
            GizmosAlpha(0.1f);
            int max = Mathf.FloorToInt(triggerSize / 2f);
            for (int i = -max; i <= max; i++) {
                Gizmos.DrawLine(new Vector3(i, 0f, 0f), new Vector3(i, sequenceLength, 0f));
            }
            DrawLabel(Vector3.up * sequenceLength / 2f + Vector3.right * (triggerSize / 2f + 1f));
        }

        void DrawDown(float triggerSize) {
            DrawRect(Vector3.down * sequenceLength / 2f, new Vector3(triggerSize, sequenceLength, 0));
            GizmosAlpha(0.1f);
            int max = Mathf.FloorToInt(triggerSize / 2f);
            for (int i = -max; i <= max; i++) {
                Gizmos.DrawLine(new Vector3(i, -sequenceLength, 0f), new Vector3(i, 0f, 0f));
            }
            DrawLabel(Vector3.down * sequenceLength / 2f + Vector3.right * (triggerSize / 2f + 1f));
        }

        bool IsSelected => Selection.objects.Contains(gameObject);

        void OnDrawGizmos() {
            
            Sequencer sequencer = GetComponentInParent<Sequencer>();
            float triggerSize = sequencer?.triggerSize ?? 10f;
            ScrollDirection direction = sequencer?.direction ?? ScrollDirection.RIGHT;
            Gizmos.color = sequencer?.gizmosColor ?? Color.red;

            Gizmos.matrix = transform.localToWorldMatrix;
            switch (direction) {

                case ScrollDirection.RIGHT:
                DrawRight(triggerSize);
                break;

                case ScrollDirection.LEFT:
                DrawLeft(triggerSize);
                break;

                case ScrollDirection.UP:
                DrawUp(triggerSize);
                break;

                case ScrollDirection.DOWN:
                DrawDown(triggerSize);
                break;
            }
        }

        public void CreateTrigger() {
            var go = new GameObject("Trigger");
            go.AddComponent<Trigger>();
            go.transform.SetParent(transform, false);
            Selection.objects = new Object[] { go as Object };
            var sequencer = GetComponentInParent<Sequencer>();
            if (sequencer != null) {
                go.transform.localPosition = sequencer.GetScrollDirection() * -sequenceLength / 2f;
            }
        }

        public void CreateSpawner() {
            var go = new GameObject();
            go.AddComponent<Spawner>();
            go.transform.SetParent(transform, false);
            Selection.objects = new Object[] { go as Object };
            var sequencer = GetComponentInParent<Sequencer>();
            if (sequencer != null) {
                go.transform.localPosition = sequencer.GetScrollDirection() * -sequenceLength / 2f;
            }
        }

        [CustomEditor(typeof(Sequence))]
        class MyEditor : Editor {
            Sequence sequence => target as Sequence;
            public override void OnInspectorGUI() {
                base.OnInspectorGUI();
                if (GUILayout.Button("Create Trigger")) {
                    sequence.CreateTrigger();
                }
                if (GUILayout.Button("Create Spawner")) {
                    sequence.CreateSpawner();
                }
            }
        }
#endif
    }
}
