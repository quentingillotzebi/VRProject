using UnityEngine;

namespace FLOW
{
	[RequireComponent(typeof(Rigidbody))]
	[HelpURL(FlowHelper.HelpUrlPrefix + "FlowShipController")]
	[AddComponentMenu(FlowHelper.ComponentMenuPrefix + "Ship Controller")]
	public class FlowShipController : MonoBehaviour
	{
		/// <summary>The keys/fingers required to move left/right.</summary>
		public FlowInputManager.Axis TurnControls { set { turnControls = value; } get { return turnControls; } } [SerializeField] private FlowInputManager.Axis turnControls = new FlowInputManager.Axis(2, false, FlowInputManager.AxisGesture.HorizontalDrag, 1.0f, KeyCode.A, KeyCode.D, KeyCode.LeftArrow, KeyCode.RightArrow, 100.0f);

		/// <summary>The keys/fingers required to move backward/forward.</summary>
		public FlowInputManager.Axis MoveControls { set { moveControls = value; } get { return moveControls; } } [SerializeField] private FlowInputManager.Axis moveControls = new FlowInputManager.Axis(2, false, FlowInputManager.AxisGesture.HorizontalDrag, 1.0f, KeyCode.S, KeyCode.W, KeyCode.DownArrow, KeyCode.UpArrow, 100.0f);

		/// <summary>The movement speed will be multiplied by this.</summary>
		public float TurnSpeed { set { turnSpeed = value; } get { return turnSpeed; } } [SerializeField] private float turnSpeed = 1.0f;

		/// <summary>The turn speed will be multiplied by this.</summary>
		public float MoveSpeed { set { moveSpeed = value; } get { return moveSpeed; } } [SerializeField] private float moveSpeed = 1.0f;

		[System.NonSerialized]
		private Rigidbody cachedRigidbody;

		protected virtual void OnEnable()
		{
			cachedRigidbody = GetComponent<Rigidbody>();
		}

		protected virtual void FixedUpdate()
		{
			var turn = turnControls.GetValue(Time.fixedDeltaTime) * turnSpeed;
			var move = moveControls.GetValue(Time.fixedDeltaTime) * moveSpeed;
			var axis = transform.forward; axis.y = 0.0f;

			cachedRigidbody.AddTorque(0.0f, turn, 0.0f, ForceMode.Acceleration);

			cachedRigidbody.AddForce(axis * move, ForceMode.Acceleration);
		}
	}
}