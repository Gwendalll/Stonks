using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sequencer {

    [ExecuteAlways]
    public class Trigger : MonoBehaviour {

        public float radius = 0.5f;
        public float offset = 0;

        [System.NonSerialized]
        public Sequencer sequencer;
        protected Sequence sequence;
        [System.NonSerialized]
        public Vector3 sequencerPosition;
        protected Vector3 sequencerPositionOld;
        [System.NonSerialized]
        public float triggerDelta;

        [Header("Gizmos")]
        public bool showLabel = true;
        public bool alwaysDisplayLabel = false;
        public bool shortLabel = false;

        public OptionColor customColor = new OptionColor();

        protected virtual string GetName() => gameObject.name;
        protected virtual string GetHandleLabel() => GetName();

        public void SetEnabled(bool value) => enabled = value;

        public bool IsValid() => sequence != null && sequencer != null;
        Vector3 GetScrollDirection() => sequencer?.GetScrollDirection() ?? transform.right;
        public Vector3 PositionWithOffset => transform.position + GetScrollDirection() * offset;
        public Vector3 GetSequencerPosition() => sequencer?.GetSequencerPosition(PositionWithOffset) ?? Vector3.zero;
        
        public void ResetTrigger() {
            sequencerPosition = GetSequencerPosition();
            sequencerPositionOld = sequencerPosition;
        }

        protected void Init() {

            gameObject.name = GetName();

            sequencer = GetComponentInParent<Sequencer>();
            sequence = GetComponentInParent<Sequence>();
            
            ResetTrigger();

            // clean at runtime
            if (Application.isPlaying) {
                while (transform.childCount > 0) {
                    DestroyImmediate(transform.GetChild(0).gameObject);
                }
            }
        }

        public bool IsInsideSequencer() {

            if (sequencer == null) return true;

            float max = sequencer?.triggerSize / 2 ?? 0f;
            switch(sequencer.direction) {
                case ScrollDirection.RIGHT:
                case ScrollDirection.LEFT:
                return sequencerPosition.y >= -max && sequencerPosition.y <= max;
                case ScrollDirection.UP:
                case ScrollDirection.DOWN:
                return sequencerPosition.x >= -max && sequencerPosition.x <= max;
            }

            throw new System.Exception("That may not happen!");
        }

        public bool Passed() {

            if (sequencer == null) return false;

            switch(sequencer.direction) {
                case ScrollDirection.RIGHT:
                return sequencerPosition.x - radius <= 0;
                case ScrollDirection.LEFT:
                return sequencerPosition.x + radius >= 0; 
                case ScrollDirection.UP:
                return sequencerPosition.y - radius <= 0; 
                case ScrollDirection.DOWN:
                return sequencerPosition.y + radius >= 0;                 
            }

            throw new System.Exception("That may not happen!");
        }

        public bool ShouldTrigger() {
            
            if (sequencer == null) return false;

            switch(sequencer.direction) {
                case ScrollDirection.RIGHT:
                return sequencerPosition.x - radius <= 0 && sequencerPositionOld.x - radius > 0;
                case ScrollDirection.LEFT:
                return sequencerPosition.x + radius >= 0 && sequencerPositionOld.x + radius < 0;
                case ScrollDirection.UP:
                return sequencerPosition.y - radius <= 0 && sequencerPositionOld.y - radius > 0;
                case ScrollDirection.DOWN:
                return sequencerPosition.y + radius >= 0 && sequencerPositionOld.y + radius < 0;                
            }

            throw new System.Exception("That may not happen!");
        }

        public Vector3 GetTriggerPosition() {

            if (sequencer == null) return transform.position;

            var x = transform.position.x;
            var y = transform.position.y;
            var sx = sequencer.transform.position.x;
            var sy = sequencer.transform.position.y;
            switch(sequencer.direction) {
                case ScrollDirection.RIGHT:
                return new Vector3(sx + (radius + offset), y, 0f);
                case ScrollDirection.LEFT:
                return new Vector3(sx - (radius + offset), y, 0f);
                case ScrollDirection.UP:
                return new Vector3(x, sy + (radius + offset), 0f);
                case ScrollDirection.DOWN:
                return new Vector3(x, sy - (radius + offset), 0f);
            }

            throw new System.Exception("That may not happen!");
        }

        void ComputeTriggerDelta() {

            if (sequencer == null) return;
            
            switch(sequencer.direction) {
                case ScrollDirection.RIGHT:
                case ScrollDirection.LEFT:
                triggerDelta = 1f - (radius - sequencerPositionOld.x) / (sequencerPosition.x - sequencerPositionOld.x);
                return;
                case ScrollDirection.UP:
                case ScrollDirection.DOWN:
                triggerDelta = 1f - (radius - sequencerPositionOld.y) / (sequencerPosition.y - sequencerPositionOld.y);
                return;
            }

            throw new System.Exception("That may not happen!");
        }

        internal void TriggerUpdate() { 
            
            sequencerPositionOld = sequencerPosition;
            sequencerPosition = GetSequencerPosition();

            float max = sequencer?.triggerSize / 2 ?? 0f;
            if (IsInsideSequencer()) {
                if (ShouldTrigger()) {
                    if (Application.isPlaying) {
                        ComputeTriggerDelta();
                        DoTrigger();
                        SendMessage("OnTriggerSequence", this, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }

        protected virtual void DoTrigger() {

        }

        void Start() {
            Init();
        }

        void OnEnable() {
            if (Application.isPlaying) {
                Init();
                sequencer.subscribeTrigger(this);
            }
        }

        void OnDisable() {
            if (Application.isPlaying) {
                sequencer.unsubscribeTrigger(this);
            }
        }

#if UNITY_EDITOR
        void Update() {

            if (Application.isPlaying == false) {
                Init();
            }
        }
#endif

        public void DrawGizmos() {
#if UNITY_EDITOR

            bool hidePassedSpawner = sequencer?.hidePassedTriggers ?? false;

            if (hidePassedSpawner && Passed()) {
                return;
            }

            var gizmosColor = sequencer?.gizmosColor ?? Color.red;
            Gizmos.color = customColor.active ? customColor.color : gizmosColor;

            if (!enabled) {
                Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
            }
            
            bool selected = Utils.GetSelected(transform);
            var positionWithOffset = PositionWithOffset;

            Gizmos.DrawLine(transform.position, positionWithOffset);
            Gizmos.DrawSphere(transform.position, selected ? 0.15f : 0.1f);
            if (radius > 0f) {
                foreach (var (A, B) in Utils.ChordAround(positionWithOffset, radius)) {
                    Gizmos.DrawLine(A, B);
                }
            } else if (offset != 0f) {
                // draw cross
                var crossSize = 0.1f;
                var d = new Vector3(crossSize, crossSize, 0);
                Gizmos.DrawLine(positionWithOffset - d, positionWithOffset + d);
                d = new Vector3(crossSize, -crossSize, 0);
                Gizmos.DrawLine(positionWithOffset - d, positionWithOffset + d);
            }
            if (showLabel) {
                Vector3 cameraPos = Camera.current.WorldToScreenPoint(transform.position);
                if (alwaysDisplayLabel || cameraPos.z < 15f) {
                    string label = GetHandleLabel();
                    if (shortLabel) {
                        label = Utils.CapitalsOnly(label);
                    }
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Gizmos.color;
                    Handles.Label(transform.position + Vector3.right * 0.2f, label, style);
                }
            }

            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, selected ? 0.1f : 0f);
            Gizmos.DrawMesh(Utils.disc, positionWithOffset, Quaternion.identity, Vector3.one * radius / 0.5f);
#endif
        }

        void OnDrawGizmos() {
            DrawGizmos();
        }
    }
}
