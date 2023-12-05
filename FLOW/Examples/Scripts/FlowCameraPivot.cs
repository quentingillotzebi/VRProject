using UnityEngine;

namespace FLOW
{
	/// <summary>This component allows you to rotate the current GameObject based on mouse/finger drags. NOTE: To function, this component requires the <b>FlowInputManager</b> component to be in your scene.</summary>
	[HelpURL(FlowHelper.HelpUrlPrefix + "FlowCameraPivot")]
	[AddComponentMenu(FlowHelper.ComponentMenuPrefix + "Camera Pivot")]
	public class FlowCameraPivot : MonoBehaviour
	{
		/// <summary>Is this component currently listening for inputs?</summary>
		public bool Listen { set { listen = value; } get { return listen; } } [SerializeField] private bool listen = true;

		/// <summary>How quickly the rotation transitions from the current to the target value (-1 = instant).</summary>
		public float Damping { set { damping = value; } get { return damping; } } [SerializeField] private float damping = 10.0f;

		/// <summary>The keys/fingers required to pitch down/up.</summary>
		public FlowInputManager.Axis PitchControls { set { pitchControls = value; } get { return pitchControls; } } [SerializeField] private FlowInputManager.Axis pitchControls = new FlowInputManager.Axis(1, true, FlowInputManager.AxisGesture.VerticalDrag, -0.1f, KeyCode.None, KeyCode.None, KeyCode.None, KeyCode.None, 45.0f);

		/// <summary>The keys/fingers required to yaw left/right.</summary>
		public FlowInputManager.Axis YawControls { set { yawControls = value; } get { return yawControls; } } [SerializeField] private FlowInputManager.Axis yawControls = new FlowInputManager.Axis(1, true, FlowInputManager.AxisGesture.HorizontalDrag, 0.1f, KeyCode.None, KeyCode.None, KeyCode.None, KeyCode.None, 45.0f);

		[System.NonSerialized]
		private Vector3 remainingDelta;

		protected virtual void OnEnable()
		{
			FlowInputManager.EnsureThisComponentExists();
		}

		protected virtual void Update()
		{
			if (listen == true)
			{
				AddToDelta();
			}

			DampenDelta();
		}

		private void AddToDelta()
		{
			remainingDelta.x += pitchControls.GetValue(Time.deltaTime);
			remainingDelta.y += yawControls  .GetValue(Time.deltaTime);
		}

		private void DampenDelta()
		{
			// Dampen remaining delta
			var factor   = FlowHelper.DampenFactor(damping, Time.deltaTime);
			var newDelta = Vector3.Lerp(remainingDelta, Vector3.zero, factor);

			// Rotate by difference
			var euler = transform.localEulerAngles;

			euler.x = -Mathf.DeltaAngle(euler.x, 0.0f);

			euler += remainingDelta - newDelta;

			euler.x = Mathf.Clamp(euler.x, -89.0f, 89.0f);

			transform.localEulerAngles = euler;

			// Update remaining
			remainingDelta = newDelta;
		}
	}
}

#if UNITY_EDITOR
namespace FLOW
{
	using TARGET = FlowCameraPivot;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class FlowCameraPivot_Editor : FlowEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("listen", "Is this component currently listening for inputs?");
			Draw("damping", "How quickly the rotation transitions from the current to the target value (-1 = instant).");

			Separator();

			Draw("pitchControls", "The keys/fingers required to pitch down/up.");
			Draw("yawControls", "The keys/fingers required to yaw left/right.");
		}
	}
}
#endif