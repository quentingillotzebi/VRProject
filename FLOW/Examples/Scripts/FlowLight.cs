using UnityEngine;

namespace FLOW
{
	/// <summary>This component will change the light intensity based on the current renderer.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(Light))]
	[HelpURL(FlowHelper.HelpUrlPrefix + "FlowLight")]
	[AddComponentMenu(FlowHelper.ComponentMenuPrefix + "Light")]
	public class FlowLight : MonoBehaviour
	{
		/// <summary>All light values will be multiplied by this before use.</summary>
		public float Multiplier { set { multiplier = value; } get { return multiplier; } } [SerializeField] private float multiplier = 1.0f;

		/// <summary>This allows you to control the intensity of the attached light when using the <b>Standard</b> rendering pipeline.
		/// -1 = The attached light intensity will not be modified.</summary>
		public float IntensityInStandard { set  { intensityInStandard = value; } get { return intensityInStandard; } } [SerializeField] private float intensityInStandard = 1.0f;

		/// <summary>This allows you to control the intensity of the attached light when using the <b>URP</b> rendering pipeline.
		/// -1 = The attached light intensity will not be modified.</summary>
		public float IntensityInURP { set  { intensityInURP = value; } get { return intensityInURP; } } [SerializeField] private float intensityInURP = 1.0f;

		/// <summary>This allows you to control the intensity of the attached light when using the <b>HDRP</b> rendering pipeline.
		/// -1 = The attached light intensity will not be modified.</summary>
		public float IntensityInHDRP { set  { intensityInHDRP = value; } get { return intensityInHDRP; } } [SerializeField] private float intensityInHDRP = 120000.0f;

		[System.NonSerialized]
		private Light cachedLight;

		[System.NonSerialized]
		private bool cachedLightSet;

		public Light CachedLight
		{
			get
			{
				if (cachedLightSet == false)
				{
					cachedLight    = GetComponent<Light>();
					cachedLightSet = true;
				}

				return cachedLight;
			}
		}

		protected virtual void Update()
		{
			var pipe = FlowShaderBundle.DetectProjectPipeline();

			if (FlowShaderBundle.IsStandard(pipe) == true)
			{
				ApplyIntensity(intensityInStandard);
			}
			else if (FlowShaderBundle.IsURP(pipe) == true)
			{
				ApplyIntensity(intensityInURP);
			}
			else if (FlowShaderBundle.IsHDRP(pipe) == true)
			{
				ApplyIntensity(intensityInHDRP);
			}
		}

		private void ApplyIntensity(float intensity)
		{
			if (intensity >= 0.0f)
			{
				if (cachedLightSet == false)
				{
					cachedLight    = GetComponent<Light>();
					cachedLightSet = true;
				}

				cachedLight.intensity = intensity * multiplier;
			}
		}
	}
}

#if UNITY_EDITOR
namespace FLOW
{
	using UnityEditor;
	using TARGET = FlowLight;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class P3dLight_Editor : FlowEditor
	{
		protected override void OnInspector()
		{
			Draw("multiplier", "All light values will be multiplied by this before use.");
			Draw("intensityInStandard", "This allows you to control the intensity of the attached light when using the Standard rendering pipeline.\n\n-1 = The attached light intensity will not be modified.");
			Draw("intensityInURP", "This allows you to control the intensity of the attached light when using the URP rendering pipeline.\n\n-1 = The attached light intensity will not be modified.");
			Draw("intensityInHDRP", "This allows you to control the intensity of the attached light when using the HDRP rendering pipeline.\n\n-1 = The attached light intensity will not be modified.");
		}
	}
}
#endif