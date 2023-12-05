using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace FLOW
{
	/// <summary>This component allows you to modify a small area of a fluid. For example, to add fluid, remove fluid, etc.</summary>
	[HelpURL(FlowHelper.HelpUrlPrefix + "FlowModifier")]
	[AddComponentMenu(FlowHelper.ComponentMenuPrefix + "Modifier")]
	public class FlowModifier : MonoBehaviour
	{
		[System.Serializable] public class FloatEvent : UnityEvent<float> {}

		public enum ApplyType
		{
			Manually,
			Once,
			Continuously
		}

		public enum HeightType
		{
			AboveAndBelow,
			Below,
			Above
		}

		public enum ModeType
		{
			AddFluid             = 10,
			AddFluidClip         = 11,
			AddFluidClipInv      = 12,
			AddFluidBelow        = 13,
			RemoveFluid          = 20,
			RemoveFluidClip      = 21,
			RemoveFluidAbove     = 22,
			RemoveFluidAboveClip = 23,
			AddForce             = 30,
			AddForceUniform      = 31,
			DampenForce          = 40,
			AddFoam              = 50,
			AddFoamMax           = 51,
			RemoveFoam           = 60,
			ChangeColor          = 70
		}

		/// <summary>This allows you to set the size of the modifier boundary in local space.
		/// NOTE: The Y value is ignored.</summary>
		public Vector3 Size { set { size = value; } get { return size; } } [SerializeField] private Vector3 size = new Vector3(1.0f, 0.0f, 1.0f);

		/// <summary>The strength of this modifier will be multiplied by a channel of this texture specified by the <b>ShapeChannel</b> setting.</summary>
		public Texture Shape { set { shape = value; } get { return shape; } } [SerializeField] private Texture shape;

		/// <summary>This allows you to choose which channel from the <b>Shape</b> texture will be used.</summary>
		public FlowChannel ShapeChannel { set { shapeChannel = value; } get { return shapeChannel; } } [SerializeField] private FlowChannel shapeChannel = FlowChannel.Alpha;

		/// <summary>Should the modifier's boundary be centered, or start from the corner</summary>
		public bool Center { set { center = value; } get { return center; } } [SerializeField] private bool center = true;

		/// <summary>How often should this component modify the underlying fluid?
		/// Manually = You must manually call the <b>ApplyNow</b> method from code or inspector event.
		/// Once = The modifier will apply on the first frame after it gets activated.
		/// Continuously = This modifier will apply every time the underlying fluid updates.</summary>
		public ApplyType Apply { set { apply = value; } get { return apply; } } [SerializeField] private ApplyType apply = ApplyType.Continuously;

		/// <summary>This allows you to choose how this component will modify the underlying fluid simulation.
		/// AddFluid = Fluid will be added under and above this modifier's local XZ position.
		/// AddFluidClip = Like <b>AddFluid</b>, but areas of the modifier that are underground will be ignored.
		/// AddFluidClipInv = Like <b>AddFluid</b>, but areas of the modifier that are overground will be ignored.
		/// RemoveFluid = Fluid will be removed under and above this modifier's local XZ position.
		/// RemoveFluidClip = Like <b>RemoveFluid</b>, but areas of the modifier that are underground will be ignored.
		/// RemoveFluidAbove = Like <b>RemoveFluid</b>, but the removal will stop once the fluid level reaches that of the modifier's Y position.
		/// RemoveFluidAboveClip = Like <b>RemoveFluidAbove</b>, but areas of the modifier that are underground will be ignored.
		/// AddForce = Fluid within the boundary of this modifier will be given force based on the specified normal map.
		/// AddForceUniform = Fluid within the boundary of this modifier will be given forward (local +Z) force.
		/// DampenForce = Fluid within the boundary of this modifier will have force removed from it. A <b>Strength</b> value of 1 will result in all force being removed.
		/// AddFoam = Fluid within the boundary of this modifier will have foam added to it.
		/// AddFoamMax = Like <b>AddFoam</b>, but the foam will only increase if the added amount is greater than the current amount.
		/// RemoveFoam = Fluid within the boundary of this modifier will have foam removed from it.
		/// ChangeColor = Fluid within the boundary of this modifier will have its color transition toward the specified color.</summary>
		public ModeType Mode { set { mode = value; } get { return mode; } } [SerializeField] private ModeType mode;

		/// <summary>This modifier will modify fluids above this height in local space.</summary>
		public float HeightMin { set { heightMin = value; } get { return heightMin; } } [SerializeField] private float heightMin;

		/// <summary>This modifier will modify fluids below this height in local space.</summary>
		public float HeightMax { set { heightMax = value; } get { return heightMax; } } [SerializeField] private float heightMax = 1.0f;

		/// <summary>When using one of the <b>AddFluid</b> modes, this allows you to specify the fluid properties that get added.</summary>
		public FlowFluid Fluid { set { fluid = value; } get { return fluid; } } [SerializeField] private FlowFluid fluid;

		/// <summary>When using <b>Mode = AddForce/Uniform</b>, this allows you to specify the direction of the force relative to the forward direction of the modifier.
		/// 0 = Forward.
		/// 90 = Right.
		/// 180 = Back.
		/// 270 = Left.</summary>
		public float Angle { set { angle = value; } get { return angle; } } [SerializeField] private float angle;

		/// <summary>When using <b>Mode = AddForce</b>, this allows you to specify the fluid properties that get added.</summary>
		public Texture Directions { set { directions = value; } get { return directions; } } [SerializeField] private Texture directions;

		/// <summary>When using <b>Mode = ChangeColor</b>, this allows you to specify the target color.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>When using <b>Mode = ChangeColor</b>, this allows you to specify which channels in the target color will be used.</summary>
		public Vector4 ColorChannels { set { colorChannels = value; } get { return colorChannels; } } [SerializeField] private Vector4 colorChannels = Vector4.one;

		/// <summary>The region modification strength will be multiplied by this amount.</summary>
		public float Strength { set { strength = value; } get { return strength; } } [SerializeField] private float strength = 1.0f;

		/// <summary>After this component applies its changes to the scene, it will invoke this event.
		/// Int = Amount of times it was applied.</summary>
		public FloatEvent OnApplied { get { if (onApplied == null) onApplied = new FloatEvent(); return onApplied; } } [SerializeField] private FloatEvent onApplied;

		/// <summary>This stores all activate and enabled <b>FlowModifier</b> instances in the scene.</summary>
		public static LinkedList<FlowModifier> Instances { get { return instances; } } private static LinkedList<FlowModifier> instances = new LinkedList<FlowModifier>(); private LinkedListNode<FlowModifier> instanceNode;

		private bool primed = true;
		
		private static Material cachedMaterial_Copy;
		private static Material cachedMaterial_AddFluid;
		private static Material cachedMaterial_RemoveFluid;
		private static Material cachedMaterial_AddForce;
		private static Material cachedMaterial_DampenForce;
		private static Material cachedMaterial_AddFoam;
		private static Material cachedMaterial_RemoveFoam;
		private static Material cachedMaterial_ChangeColor;

		public bool UsesHeightMinMax
		{
			get
			{
				switch (mode)
				{
					case ModeType.AddFluid: return false;
					case ModeType.AddFluidClip: return false;
					case ModeType.AddFluidClipInv: return false;
					case ModeType.AddFluidBelow: return false;
					case ModeType.RemoveFluid: return false;
					case ModeType.RemoveFluidClip: return false;
					case ModeType.RemoveFluidAbove: return false;
					case ModeType.RemoveFluidAboveClip: return false;
				}

				return true;
			}
		}

		private Vector4 ShapeChannelVector
		{
			get
			{
				switch (shapeChannel)
				{
					case FlowChannel.Red  : return new Vector4(1,0,0,0);
					case FlowChannel.Green: return new Vector4(0,1,0,0);
					case FlowChannel.Blue : return new Vector4(0,0,1,0);
					case FlowChannel.Alpha: return new Vector4(0,0,0,1);
				}

				return Vector4.zero;
			}
		}

		protected virtual void OnEnable()
		{
			instanceNode = instances.AddLast(this);

			primed = true;
		}

		protected virtual void OnDisable()
		{
			instances.Remove(instanceNode); instanceNode = null;
		}

		protected virtual void Update()
		{
			if (apply == ApplyType.Once && primed == true)
			{
				primed = false;

				ApplyNow();
			}
		}

		/// <summary>This method will apply this region to all volumes in the scene using the specified <b>Strength</b> value.</summary>
		[ContextMenu("Apply Now")]
		public void ApplyNow()
		{
			ApplyNow(1.0f);
		}

		public void ApplyNow(float multiplier)
		{
			if (multiplier > 0.0f && strength != 0)
			{
				var bounds          = CalculateLocalBounds();
				var heightMid       = (heightMin + heightMax) * 0.5f;
				var heightDiff      = (heightMax - heightMin) * 0.5f;
				var modifierMatrix  = transform.localToWorldMatrix * Matrix4x4.Translate(new Vector3(bounds.min.x, 0.0f, bounds.min.z)) * Matrix4x4.Scale(bounds.size);
				var modifierInverse = Matrix4x4.Scale(new Vector3(1.0f, heightDiff != 0.0f ? 1.0f / heightDiff : 0.0f, 1.0f)) * Matrix4x4.Translate(new Vector3(0.0f, -heightMid, 0.0f)) * transform.worldToLocalMatrix;

				foreach (var fluid in FlowSimulation.Instances)
				{
					if (fluid.Activated == true)
					{
						switch (mode)
						{
							case ModeType.AddFluid:
							{
								if (fluid != null)
								{
									if (cachedMaterial_AddFluid == null) cachedMaterial_AddFluid = new Material(Resources.Load<Shader>("FLOW/Modifier_AddFluid"));

									ApplyNow(multiplier, modifierMatrix, modifierInverse, fluid, fluid.CurrentFlowDataC, fluid.CurrentFlowDataD, fluid.CurrentFlowDataE, fluid.CurrentFlowDataF, cachedMaterial_AddFluid, 0);
								}
							}
							break;

							case ModeType.AddFluidClip:
							{
								if (fluid != null)
								{
									if (cachedMaterial_AddFluid == null) cachedMaterial_AddFluid = new Material(Resources.Load<Shader>("FLOW/Modifier_AddFluid"));

									ApplyNow(multiplier, modifierMatrix, modifierInverse, fluid, fluid.CurrentFlowDataC, fluid.CurrentFlowDataD, fluid.CurrentFlowDataE, fluid.CurrentFlowDataF, cachedMaterial_AddFluid, 1);
								}
							}
							break;

							case ModeType.AddFluidClipInv:
							{
								if (fluid != null)
								{
									if (cachedMaterial_AddFluid == null) cachedMaterial_AddFluid = new Material(Resources.Load<Shader>("FLOW/Modifier_AddFluid"));

									ApplyNow(multiplier, modifierMatrix, modifierInverse, fluid, fluid.CurrentFlowDataC, fluid.CurrentFlowDataD, fluid.CurrentFlowDataE, fluid.CurrentFlowDataF, cachedMaterial_AddFluid, 2);
								}
							}
							break;

							case ModeType.AddFluidBelow:
							{
								if (fluid != null)
								{
									if (cachedMaterial_AddFluid == null) cachedMaterial_AddFluid = new Material(Resources.Load<Shader>("FLOW/Modifier_AddFluid"));

									ApplyNow(multiplier, modifierMatrix, modifierInverse, fluid, fluid.CurrentFlowDataC, fluid.CurrentFlowDataD, fluid.CurrentFlowDataE, fluid.CurrentFlowDataF, cachedMaterial_AddFluid, 3);
								}
							}
							break;

							case ModeType.RemoveFluid:
							{
								if (cachedMaterial_RemoveFluid == null) cachedMaterial_RemoveFluid = new Material(Resources.Load<Shader>("FLOW/Modifier_RemoveFluid"));

								ApplyNow(multiplier, modifierMatrix, modifierInverse, fluid, fluid.CurrentFlowDataC, cachedMaterial_RemoveFluid, 0);
							}
							break;

							case ModeType.RemoveFluidClip:
							{
								if (cachedMaterial_RemoveFluid == null) cachedMaterial_RemoveFluid = new Material(Resources.Load<Shader>("FLOW/Modifier_RemoveFluid"));

								ApplyNow(multiplier, modifierMatrix, modifierInverse, fluid, fluid.CurrentFlowDataC, cachedMaterial_RemoveFluid, 1);
							}
							break;

							case ModeType.RemoveFluidAbove:
							{
								if (cachedMaterial_RemoveFluid == null) cachedMaterial_RemoveFluid = new Material(Resources.Load<Shader>("FLOW/Modifier_RemoveFluid"));

								ApplyNow(multiplier, modifierMatrix, modifierInverse, fluid, fluid.CurrentFlowDataC, cachedMaterial_RemoveFluid, 2);
							}
							break;

							case ModeType.RemoveFluidAboveClip:
							{
								if (cachedMaterial_RemoveFluid == null) cachedMaterial_RemoveFluid = new Material(Resources.Load<Shader>("FLOW/Modifier_RemoveFluid"));

								ApplyNow(multiplier, modifierMatrix, modifierInverse, fluid, fluid.CurrentFlowDataC, cachedMaterial_RemoveFluid, 3);
							}
							break;

							case ModeType.AddForce:
							{
								if (directions != null)
								{
									if (cachedMaterial_AddForce == null) cachedMaterial_AddForce = new Material(Resources.Load<Shader>("FLOW/Modifier_AddForce"));

									ApplyNow(multiplier, modifierMatrix, modifierInverse, fluid, fluid.CurrentFlowDataB, cachedMaterial_AddForce, 0);
								}
							}
							break;

							case ModeType.AddForceUniform:
							{
								if (cachedMaterial_AddForce == null) cachedMaterial_AddForce = new Material(Resources.Load<Shader>("FLOW/Modifier_AddForce"));

								ApplyNow(multiplier, modifierMatrix, modifierInverse, fluid, fluid.CurrentFlowDataB, cachedMaterial_AddForce, 1);
							}
							break;

							case ModeType.DampenForce:
							{
								if (cachedMaterial_DampenForce == null) cachedMaterial_DampenForce = new Material(Resources.Load<Shader>("FLOW/Modifier_DampenForce"));

								ApplyNow(multiplier, modifierMatrix, modifierInverse, fluid, fluid.CurrentFlowDataB, cachedMaterial_DampenForce, 0);
							}
							break;

							case ModeType.AddFoam:
							{
								if (cachedMaterial_AddFoam == null) cachedMaterial_AddFoam = new Material(Resources.Load<Shader>("FLOW/Modifier_AddFoam"));

								ApplyNow(multiplier, modifierMatrix, modifierInverse, fluid, fluid.CurrentFlowDataF, cachedMaterial_AddFoam, 0);
							}
							break;

							case ModeType.AddFoamMax:
							{
								if (cachedMaterial_AddFoam == null) cachedMaterial_AddFoam = new Material(Resources.Load<Shader>("FLOW/Modifier_AddFoam"));

								ApplyNow(1.0f, modifierMatrix, modifierInverse, fluid, fluid.CurrentFlowDataF, cachedMaterial_AddFoam, 1); // NOTE: Override multiplier to 1 because it's not being applied continuously
							}
							break;

							case ModeType.RemoveFoam:
							{
								if (cachedMaterial_RemoveFoam == null) cachedMaterial_RemoveFoam = new Material(Resources.Load<Shader>("FLOW/Modifier_RemoveFoam"));

								ApplyNow(multiplier, modifierMatrix, modifierInverse, fluid, fluid.CurrentFlowDataF, cachedMaterial_RemoveFoam, 0);
							}
							break;

							case ModeType.ChangeColor:
							{
								if (cachedMaterial_ChangeColor == null) cachedMaterial_ChangeColor = new Material(Resources.Load<Shader>("FLOW/Modifier_ChangeColor"));

								ApplyNow(multiplier, modifierMatrix, modifierInverse, fluid, fluid.CurrentFlowDataD, cachedMaterial_ChangeColor, 0);
							}
							break;
						}
						
					}
				}

				if (onApplied != null)
				{
					onApplied.Invoke(multiplier);
				}
			}
		}

		/// <summary>This allows you create a new GameObject with the <b>FlowModifier</b> component attached.</summary>
		public static FlowModifier Create(int layer = 0, Transform parent = null)
		{
			return Create(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
		}

		/// <summary>This allows you create a new GameObject with the <b>FlowModifier</b> component attached.</summary>
		public static FlowModifier Create(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
		{
			return FlowHelper.CreateGameObject("Modifier", layer, parent, localPosition, localRotation, localScale).AddComponent<FlowModifier>();
		}

		private void ApplyNow(float multiplier, Matrix4x4 modifierMatrix, Matrix4x4 modifierInverse, FlowSimulation simulation, RenderTexture textureA, Material cachedMaterial, int pass)
		{
			FlowBuffer.Size1.Set(0, textureA);

			ApplyNow(multiplier, modifierMatrix, modifierInverse, simulation, FlowBuffer.Size1, cachedMaterial, pass);
		}

		private void ApplyNow(float multiplier, Matrix4x4 modifierMatrix, Matrix4x4 modifierInverse, FlowSimulation simulation, RenderTexture textureA, RenderTexture textureB, RenderTexture textureC, RenderTexture textureD, Material cachedMaterial, int pass)
		{
			FlowBuffer.Size4.Set(0, textureA);
			FlowBuffer.Size4.Set(1, textureB);
			FlowBuffer.Size4.Set(2, textureC);
			FlowBuffer.Size4.Set(3, textureD);

			ApplyNow(multiplier, modifierMatrix, modifierInverse, simulation, FlowBuffer.Size4, cachedMaterial, pass);
		}

		private void ApplyNow(float multiplier, Matrix4x4 modifierMatrix, Matrix4x4 modifierInverse, FlowSimulation simulation, FlowBuffer buffer, Material cachedMaterial, int pass)
		{
			cachedMaterial.SetMatrix(FlowShader._ModifierMatrix, modifierMatrix);
			cachedMaterial.SetMatrix(FlowShader._ModifierInverse, modifierInverse);
			cachedMaterial.SetFloat(FlowShader._ModifierStrength, strength * multiplier);
			cachedMaterial.SetVector(FlowShader._ModifierChannel, ShapeChannelVector);
			cachedMaterial.SetTexture(FlowShader._ModifierShape, shape != null ? shape : Texture2D.whiteTexture);

			if (mode == ModeType.AddForce)
			{
				cachedMaterial.SetTexture(FlowShader._ModifierNormal, directions);
			}

			if (mode == ModeType.AddFluid || mode == ModeType.AddFluidClip || mode == ModeType.AddFluidClipInv || mode == ModeType.AddFluidBelow)
			{
				if (fluid != null)
				{
					var foam = 0.0f;

					cachedMaterial.SetVector(FlowShader._ModifierRGBA, fluid.Color);
					cachedMaterial.SetVector(FlowShader._ModifierESMV, new Vector4(fluid.Emission, fluid.Smoothness, fluid.Metallic, fluid.Viscosity));
					cachedMaterial.SetVector(FlowShader._ModifierF123, new Vector4(foam, fluid.Custom1, fluid.Custom2, fluid.Custom3));
				}
			}

			if (mode == ModeType.AddForceUniform)
			{
				cachedMaterial.SetFloat(FlowShader._ModifierAngle, (-Vector3.SignedAngle(transform.forward, Vector3.forward, Vector3.up) + angle) * Mathf.Deg2Rad);
			}

			if (mode == ModeType.ChangeColor)
			{
				cachedMaterial.SetVector(FlowShader._ModifierRGBA, color);
				cachedMaterial.SetVector(FlowShader._ModifierChannels, colorChannels);
			}

			simulation.SetVariables(cachedMaterial);

			buffer.SetRenderTargets();

			Graphics.Blit(null, cachedMaterial, pass);

			for (var i = 0; i < buffer.Count; i++)
			{
				CopyBack(simulation, modifierMatrix, buffer.SourceTextures[i], buffer.TempTextures[i]);
			}

			buffer.ReleaseAll();
		}

		private void CopyBack(FlowSimulation simulation, Matrix4x4 modifierMatrix, RenderTexture target, RenderTexture tempBuffer)
		{
			if (cachedMaterial_Copy == null) cachedMaterial_Copy = new Material(Resources.Load<Shader>("FLOW/Modifier_Copy"));

			cachedMaterial_Copy.SetMatrix(FlowShader._ModifierMatrix, modifierMatrix);
			cachedMaterial_Copy.SetTexture(FlowShader._ModifierBuffer, tempBuffer);

			simulation.SetVariables(cachedMaterial_Copy);

			Graphics.Blit(null, target, cachedMaterial_Copy, 0);
		}

		private Bounds CalculateLocalBounds()
		{
			var boundsCenter = Vector3.zero;
			var boundsSize   = new Vector3(size.x, 0.0f, size.z);

			if (center == false)
			{
				boundsCenter.x = boundsSize.x * 0.5f;
				boundsCenter.z = boundsSize.z * 0.5f;
			}

			return new Bounds(boundsCenter, boundsSize);
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;

			if (UsesHeightMinMax == true)
			{
				DrawGizmosHeight(heightMin);
				DrawGizmosHeight(heightMax);
			}
			else
			{
				DrawGizmosHeight(0.0f);
			}
		}

		private void DrawGizmosHeight(float height)
		{
			var bounds  = CalculateLocalBounds();
			var corner  = bounds.min; corner += Vector3.up * height;
			var right   = Vector3.right   * bounds.size.x;
			var forward = Vector3.forward * bounds.size.z;

			var pointA = corner;
			var pointB = corner + right;
			var pointC = corner + forward;
			var pointD = corner + right + forward;

			Gizmos.DrawLine(pointA, pointB);
			Gizmos.DrawLine(pointA, pointC);
			Gizmos.DrawLine(pointD, pointB);
			Gizmos.DrawLine(pointD, pointC);
		}
#endif
	}
}

#if UNITY_EDITOR
namespace FLOW
{
	using UnityEditor;
	using TARGET = FlowModifier;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class FlowModifier_Editor : FlowEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => System.Enum.IsDefined(typeof(TARGET.ModeType), t.Mode) == false));
				Draw("mode", "This allows you to choose how this component will modify the underlying fluid simulation.\n\nAddFluid = Fluid will be added under and above this modifier's local XZ position.\n\nAddFluidClip = Like <b>AddFluid</b>, but areas of the modifier that are underground will be ignored.\n\nAddFluidClipInv = Like <b>AddFluid</b>, but areas of the modifier that are overground will be ignored.\n\nRemoveFluid = Fluid will be removed under and above this modifier's local XZ position.\n\nRemoveFluidClip = Like <b>RemoveFluid</b>, but areas of the modifier that are underground will be ignored.\n\nRemoveFluidAbove = Like <b>RemoveFluid</b>, but the removal will stop once the fluid level reaches that of the modifier's Y position.\n\nRemoveFluidAboveClip = Like <b>RemoveFluidAbove</b>, but areas of the modifier that are underground will be ignored.\n\nAddForce = Fluid within the boundary of this modifier will be given force based on the specified normal map.\n\nAddForceUniform = Fluid within the boundary of this modifier will be given forward (local +Z) force.\n\nDampenForce = Fluid within the boundary of this modifier will have force removed from it. A <b>Strength</b> value of 1 will result in all force being removed.\n\nAddFoam = Fluid within the boundary of this modifier will have foam added to it.\n\nAddFoamMax = Like <b>AddFoam</b>, but the foam will only increase if the added amount is greater than the current amount.\n\nRemoveFoam = Fluid within the boundary of this modifier will have foam removed from it.\n\nChangeColor = Fluid within the boundary of this modifier will have its color transition toward the specified color.");
			EndError();

			Separator();

			BeginError(Any(tgts, t => t.Size.x <= 0.0f || t.Size.z <= 0.0f));
				Draw("size", "This allows you to set the size of the modifier boundary in local space.\n\nNOTE: The Y value is ignored.");
			EndError();
			EditorGUILayout.BeginHorizontal();
				Draw("shape", "The strength of this modifier will be multiplied by a channel of this texture specified by the <b>ShapeChannel</b> setting.");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("shapeChannel"), GUIContent.none, GUILayout.Width(60));
			EditorGUILayout.EndHorizontal();
			Draw("center", "Should the modifier's boundary be centered, or start from the corner?");

			if (Any(tgts, t => t.UsesHeightMinMax == true))
			{
				Separator();

				BeginError(Any(tgts, t => t.HeightMax <= t.HeightMin));
					Draw("heightMin", "This modifier will modify fluids above this height in local space.");
					Draw("heightMax", "This modifier will modify fluids below this height in local space.");
				EndError();
			}

			Separator();

			Draw("apply", "How often should this component modify the underlying fluid?\n\nManually = You must manually call the <b>ApplyNow</b> method from code or inspector event.\n\nOnce = The modifier will apply on the first frame after it gets activated.\n\nContinuously = This modifier will apply every time the underlying fluid updates.");

			if (Any(tgts, t => t.Mode == TARGET.ModeType.AddFluid || t.Mode == TARGET.ModeType.AddFluidClip || t.Mode == TARGET.ModeType.AddFluidClipInv || t.Mode == TARGET.ModeType.AddFluidBelow))
			{
				BeginError(Any(tgts, t => t.Fluid == null));
					Draw("fluid", "When using one of the <b>AddFluid</b> modes, this allows you to specify the fluid properties that get added.");
				EndError();
			}
			if (Any(tgts, t => t.Mode == TARGET.ModeType.AddForce))
			{
				Draw("directions", "When using <b>Mode = AddForce</b>, this allows you to specify the fluid properties that get added.");
			}
			if (Any(tgts, t => t.Mode == TARGET.ModeType.AddForce || t.Mode == TARGET.ModeType.AddForceUniform))
			{
				Draw("angle", "When using <b>Mode = AddForce/Direction</b>, this allows you to specify the direction of the force relative to the forward direction of the modifier.\n\n0 = Forward.\n\n90 = Right.\n\n180 = Back.\n\n270 = Left.");
			}
			if (Any(tgts, t => t.Mode == TARGET.ModeType.ChangeColor))
			{
				Draw("color", "When using <b>Mode = ChangeColor</b>, this allows you to specify the target color.");
				DrawVector4("colorChannels", "When using <b>Mode = ChangeColor</b>, this allows you to specify which channels in the target color will be used.");
			}

			Draw("strength", "The region modification strength will be multiplied by this amount.");

			Separator();

			Draw("onApplied");
		}

		[MenuItem(FlowHelper.GameObjectMenuPrefix + "Modifier", false, 10)]
		public static void CreateMenuItem()
		{
			var parent   = FlowHelper.GetSelectedParent();
			var instance = FlowModifier.Create(parent != null ? parent.gameObject.layer : 0, parent);

			FlowHelper.SelectAndPing(instance);
		}
	}
}
#endif