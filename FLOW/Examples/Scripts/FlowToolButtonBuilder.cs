using UnityEngine;
using UnityEngine.UI;

namespace FLOW
{
	/// <summary>
	/// This component allows you to quickly build a UI button to activate only this GameObject when clicked.</summary>
	[HelpURL(FlowHelper.HelpUrlPrefix + "FlowToolButtonBuilder")]
	[AddComponentMenu(FlowHelper.ComponentMenuPrefix + "Tool Button Builder")]
	public class FlowToolButtonBuilder : MonoBehaviour
	{
		/// <summary>The built button will be based on this prefab.</summary>
		public FlowToolButton ButtonPrefab { set { buttonPrefab = value; } get { return buttonPrefab; } } [SerializeField] private FlowToolButton buttonPrefab;

		/// <summary>The built button will be placed under this transform.</summary>
		public RectTransform ButtonRoot { set { buttonRoot = value; } get { return buttonRoot; } } [SerializeField] private RectTransform buttonRoot;

		/// <summary>The icon given to this button.</summary>
		public Sprite Icon { set { icon = value; } get { return icon; } } [SerializeField] private Sprite icon;

		/// <summary>The icon will be tinted by this.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>Use a different name for the button text?</summary>
		public string OverrideName { set { overrideName = value; } get { return overrideName; } } [SerializeField] [Multiline(3)] private string overrideName;

		[SerializeField]
		private FlowToolButton clone;

		[ContextMenu("Build")]
		public void Build()
		{
			if (clone != null)
			{
				DestroyImmediate(clone.gameObject);
			}

			if (buttonPrefab != null)
			{
				clone = DoInstantiate();

				clone.name   = name;
				clone.Target = transform;

				var image = clone.GetComponent<Image>();

				if (image != null)
				{
					image.sprite = icon;
					image.color  = color;
				}

				var title = clone.GetComponentInChildren<Text>();

				if (title != null)
				{
					title.text = string.IsNullOrEmpty(overrideName) == false ? overrideName : name;
				}
			}
		}

		[ContextMenu("Build All")]
		public void BuildAll()
		{
			foreach (var builder in transform.parent.GetComponentsInChildren<FlowToolButtonBuilder>(true))
			{
				builder.Build();
			}
		}

		private FlowToolButton DoInstantiate()
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false)
			{
				return (FlowToolButton)UnityEditor.PrefabUtility.InstantiatePrefab(buttonPrefab, buttonRoot);
			}
#endif
			return Instantiate(buttonPrefab, buttonRoot, false);
		}
	}
}

#if UNITY_EDITOR
namespace FLOW
{
	using UnityEditor;
	using TARGET = FlowToolButtonBuilder;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class FlowToolButtonBuilder_Editor : FlowEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("buttonPrefab", "The built button will be based on this prefab.");
			Draw("buttonRoot", "The built button will be placed under this transform.");

			Separator();

			Draw("icon", "The icon given to this button.");
			Draw("color", "The icon will be tinted by this.");
			Draw("overrideName", "Use a different name for the button text?");

			Separator();

			if (Button("Build All") == true)
			{
				Undo.RecordObjects(tgts, "Build All");

				tgt.BuildAll();
			}
		}
	}
}
#endif