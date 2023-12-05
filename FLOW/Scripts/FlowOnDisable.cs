﻿using UnityEngine;
using UnityEngine.Events;

namespace FLOW
{
	/// <summary>This component allows you to invoke an event when this component gets disabled.</summary>
	[HelpURL(FlowHelper.HelpUrlPrefix + "FlowOnDisable")]
	[AddComponentMenu(FlowHelper.ComponentMenuPrefix + "On Disable")]
	public class FlowOnDisable : MonoBehaviour
	{
		/// <summary>The event that will be invoked.</summary>
		public UnityEvent Action { get { return action; } } [SerializeField] private UnityEvent action = null;

		protected virtual void OnDisable()
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
	using TARGET = FlowOnDisable;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class FlowOnDisable_Editor : FlowEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("action");
		}
	}
}
#endif