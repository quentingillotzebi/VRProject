using UnityEngine;
using System.Collections.Generic;

namespace FLOW
{
	/// <summary>This component sends wetness data to all materials on the current GameObject.</summary>
	[HelpURL(FlowHelper.HelpUrlPrefix + "FlowWetness")]
	[AddComponentMenu(FlowHelper.ComponentMenuPrefix + "Wetness")]
	public class FlowWetness : MonoBehaviour
	{
		/// <summary>The simulation whose data will be set on this GameObject's materials.
		/// None/null = Use the closest simulation.</summary>
		public FlowSimulation Simulation { set { simulation = value; } get { return simulation; } } [SerializeField] private FlowSimulation simulation;

		/// <summary>The wetness data will be applied to all materials in these renderers.</summary>
		public List<Renderer> Renderers { get { if (renderers == null) renderers = new List<Renderer>(); return renderers; } } [SerializeField] private List<Renderer> renderers;

		/// <summary>The wetness data will be applied to all these materials.</summary>
		public List<Terrain> Terrains { get { if (terrains == null) terrains = new List<Terrain>(); return terrains; } } [SerializeField] private List<Terrain> terrains;

		[System.NonSerialized]
		private Renderer cachedRenderer;

		[System.NonSerialized]
		private MaterialPropertyBlock properties;

		[System.NonSerialized]
		private FlowSimulation registeredSimulation;

		private static List<Material> tempMaterials = new List<Material>();

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Renderers.AddRange(GetComponentsInChildren<Renderer>());
		}
#endif

		protected virtual void OnEnable()
		{
			cachedRenderer = GetComponent<Renderer>();
		}

		protected virtual void OnDisable()
		{
			if (registeredSimulation != null)
			{
				registeredSimulation.OnUpdated -= HandleUpdated;
			}
		}

		protected virtual void Update()
		{
			if (registeredSimulation != null)
			{
				registeredSimulation.OnUpdated -= HandleUpdated;
			}

			registeredSimulation = FlowSimulation.FindSimulation(transform.position, simulation);

			if (registeredSimulation != null)
			{
				registeredSimulation.OnUpdated += HandleUpdated;
			}

			HandleUpdated();
		}

		private void HandleUpdated()
		{
			if (registeredSimulation != null && registeredSimulation.Activated == true)
			{
				if (properties == null)
				{
					properties = new MaterialPropertyBlock();
				}

				if (cachedRenderer != null)
				{
					cachedRenderer.GetSharedMaterials(tempMaterials);

					for (var i = 0; i < tempMaterials.Count; i++)
					{
						cachedRenderer.GetPropertyBlock(properties, i);

						registeredSimulation.SetWetnessVariables(properties);

						cachedRenderer.SetPropertyBlock(properties, i);
					}
				}

				if (terrains != null)
				{
					foreach (var terrain in terrains)
					{
						if (terrain != null)
						{
							var material = terrain.materialTemplate;

							if (material != null)
							{
								registeredSimulation.SetWetnessVariables(material);

								terrain.materialTemplate = null;
								terrain.materialTemplate = material;
							}
						}
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace FLOW
{
	using UnityEditor;
	using TARGET = FlowWetness;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class FlowWetness_Editor : FlowEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("simulation", "The simulation whose data will be set on this GameObject's materials.\n\nNone/null = Use the closest simulation.");

			Separator();

			BeginError(Any(tgts, t => t.Renderers.Exists(r => r != null) == false && t.Terrains.Exists(m => m != null) == false));
				Draw("renderers", "The wetness data will be applied to all materials in these renderers.");
				Draw("terrains", "The wetness data will be applied to all these materials.");
			EndError();
		}
	}
}
#endif