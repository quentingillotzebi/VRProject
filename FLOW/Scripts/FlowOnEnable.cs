using UnityEngine;
using UnityEngine.Events;

namespace FLOW
{
	/// <summary>This component allows you to invoke an event when this component gets enabled.</summary>
	[HelpURL(FlowHelper.HelpUrlPrefix + "FlowOnEnable")]
	[AddComponentMenu(FlowHelper.ComponentMenuPrefix + "On Enable")]
	public class FlowOnEnable : MonoBehaviour
	{
		/// <summary>The event that will be invoked.</summary>
		public UnityEvent Action { get { return action; } } [SerializeField] private UnityEvent action = null;

		protected virtual void OnEnable()
		{
			if (action != null)
			{
				action.Invoke();
			}
		}
	}
}

#if UNITY_EDITOR
namespace FLOW
{
	using UnityEditor;
	using TARGET = FlowOnEnable;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class FlowOnEnable_Editor : FlowEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("action");
		}
	}
}
#endif