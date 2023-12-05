using UnityEngine;
using UnityEngine.EventSystems;

namespace FLOW
{
	/// <summary>This component allows you to turn a UI element into a button that will activate the target GameObject, and deactivate its siblings. If the target GameObject is active, then the button will be faded in.</summary>
	[ExecuteInEditMode]
	[HelpURL(FlowHelper.HelpUrlPrefix + "FlowToolButton")]
	[AddComponentMenu(FlowHelper.ComponentMenuPrefix + "Tool Button")]
	public class FlowToolButton : MonoBehaviour, IPointerDownHandler
	{
		/// <summary>If this GameObject is active, then the button will be faded in.</summary>
		public Transform Target { set { target = value; } get { return target; } } [SerializeField] private Transform target;

		protected virtual void Update()
		{
			if (target != null)
			{
				var group = GetComponent<CanvasGroup>();

				if (group != null)
				{
					var factor = FlowHelper.DampenFactor(10.0f, Time.deltaTime);
					var alphaT = target.gameObject.activeInHierarchy == true ? 1.0f : 0.5f;

					group.alpha = Mathf.Lerp(group.alpha, alphaT, factor);
				}
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (target != null)
			{
				var parent = target.transform.parent;
				var active = target.gameObject.activeSelf;

				for (var i = 0; i < parent.childCount; i++)
				{
					parent.GetChild(i).gameObject.SetActive(false);
				}

				target.gameObject.SetActive(active == false);
			}
		}
	}
}

#if UNITY_EDITOR
namespace FLOW
{
	using UnityEditor;
	using TARGET = FlowToolButton;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class FlowToolButton_Editor : FlowEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.Target == null));
				Draw("target", "If this GameObject is active, then the button will be faded in.");
			EndError();
		}
	}
}
#endif