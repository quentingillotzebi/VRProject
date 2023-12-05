using UnityEngine;

namespace FLOW
{
	/// <summary>This component will modify the sibling <b>FlowModifier.Strength</b> setting based on the speed this GameObject moves.</summary>
	[RequireComponent(typeof(FlowModifier))]
	[HelpURL(FlowHelper.HelpUrlPrefix + "FlowModifierSpeed")]
	[AddComponentMenu(FlowHelper.ComponentMenuPrefix + "Modifier Speed")]
	public class FlowModifierSpeed : MonoBehaviour
	{
		/// <summary>When this GameObject's speed matches this value, the <b>FlowModifier.Strength</b> will be set to the specified <b>Strength</b> value.</summary>
		public float SpeedMax { set { speedMax = value; } get { return speedMax; } } [SerializeField] private float speedMax = 10.0f;

		/// <summary>The <b>FlowModifier.Strength</b> will be set to this value when this GameObject's speed matches the <b>SpeedMax</b> value.</summary>
		public float Strength { set { strength = value; } get { return strength; } } [SerializeField] private float strength = 1.0f;

		/// <summary>If the speed exceeds <b>SpeedMax</b>, should the calculations be clamped, as if the speed wasn't exceeded?</summary>
		public bool Clamp { set { clamp = value; } get { return clamp; } } [SerializeField] private bool clamp = true;

		public float Speed
		{
			get
			{
				return lastSpeed;
			}
		}

		[System.NonSerialized]
		private FlowModifier cachedModifier;

		[System.NonSerialized]
		private Vector3 lastPosition;

		[System.NonSerialized]
		private float lastSpeed;

		protected virtual void OnEnable()
		{
			cachedModifier   = GetComponent<FlowModifier>();
			lastPosition = transform.position;
		}

		protected virtual void FixedUpdate()
		{
			var newPosition = transform.position;
			var speed       = Vector3.Distance(lastPosition, newPosition) / Time.fixedDeltaTime;
			var speed01     = speedMax != 0.0f ? speed / speedMax : 0.0f;

			if (clamp == true)
			{
				speed01 = Mathf.Clamp01(speed01);
			}

			cachedModifier.Strength = strength * speed01;

			lastPosition = newPosition;
			lastSpeed    = speed;
		}
	}
}

#if UNITY_EDITOR
namespace FLOW
{
	using UnityEditor;
	using TARGET = FlowModifierSpeed;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class FlowModifierSpeed_Editor : FlowEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("speedMax", "When this GameObject's speed matches this value, the <b>FlowModifier.Strength</b> will be set to the specified <b>Strength</b> value.");
			Draw("strength", "The <b>FlowModifier.Strength</b> will be set to this value when this GameObject's speed matches the <b>SpeedMax</b> value.");
			Draw("clamp", "If the speed exceeds <b>SpeedMax</b>, should the calculations be clamped, as if the speed wasn't exceeded?");

			Separator();

			BeginDisabled();
				EditorGUILayout.FloatField("Speed", tgt.Speed);
			EndDisabled();
		}
	}
}
#endif