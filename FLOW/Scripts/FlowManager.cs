﻿using UnityEngine;
using System.Collections.Generic;

namespace FLOW
{
	/// <summary>This component updates all fluids in the scene.
	/// NOTE: For FLOW to work, this component must exist in your scene somewhere.</summary>
	[HelpURL(FlowHelper.HelpUrlPrefix + "FlowManager")]
	[AddComponentMenu(FlowHelper.ComponentMenuPrefix + "Manager")]
	public class FlowManager : MonoBehaviour
	{
		public enum UpdateType
		{
			EveryFrame,
			EveryOtherFrame
		}

		/// <summary>This allows you to control how often this simulation updates.
		/// EveryFrame = All buffers will be updated every FixedUpdate.
		/// EveryOtherFrame = All buffers will be updated every two FixedUpdate calls.</summary>
		public UpdateType UpdateMode { set { updateMode = value; } get { return updateMode; } } [SerializeField] private UpdateType updateMode;

		/// <summary>This stores all activate and enabled <b>FlowManager</b> instances in the scene.</summary>
		public static LinkedList<FlowManager> Instances { get { return instances; } } private static LinkedList<FlowManager> instances = new LinkedList<FlowManager>(); private LinkedListNode<FlowManager> instanceNode;

		private bool partiallyUpdated;

		public static void EnsureThisComponentExists()
		{
			if (Application.isPlaying == true && FindObjectOfType<FlowManager>() == null)
			{
				new GameObject(typeof(FlowManager).Name).AddComponent<FlowManager>();
			}
		}

		protected virtual void OnEnable()
		{
			instanceNode = instances.AddLast(this);
		}

		protected virtual void OnDisable()
		{
			instances.Remove(instanceNode); instanceNode = null;
		}

		protected virtual void Update()
		{
			//FlowReader.Update();
		}

		protected virtual void FixedUpdate()
		{
			if (instanceNode == instances.First)
			{
				UpdateAllSimulations(Time.fixedDeltaTime);

				UpdateAllSamples();
			}
		}

		private void UpdateAllSimulations(float delta)
		{
			if (updateMode == UpdateType.EveryFrame)
			{
				foreach (var simulation in FlowSimulation.Instances)
				{
					simulation.UpdateFluidForces();
				}

				UpdateAllModifiers(delta);

				foreach (var simulation in FlowSimulation.Instances)
				{
					simulation.UpdateFluidTransport(delta);
					simulation.UpdateParticles(delta);
				}
			}
			else if (updateMode == UpdateType.EveryOtherFrame)
			{
				foreach (var simulation in FlowSimulation.Instances)
				{
					if (partiallyUpdated == false)
					{
						simulation.UpdateFluidForces();
					}
					else
					{
						UpdateAllModifiers(delta * 2.0f);
						simulation.UpdateFluidTransport(delta * 2.0f);
					}

					simulation.UpdateParticles(delta);
				}

				partiallyUpdated = !partiallyUpdated;
			}

			foreach (var simulation in FlowSimulation.Instances)
			{
				simulation.UpdateFluidWetness(Time.fixedDeltaTime);

				simulation.NotifyUpdated();
			}
		}

		private void UpdateAllModifiers(float delta)
		{
			foreach (var modifier in FlowModifier.Instances)
			{
				if (modifier.Apply == FlowModifier.ApplyType.Continuously)
				{
					modifier.ApplyNow(delta);
				}
			}
		}

		private void UpdateAllSamples()
		{
			if (FlowReader.Ready == true)
			{
				foreach (var sample in FlowSample.Instances)
				{
					var worldPosition = sample.transform.position;
					var simulation    = FlowSimulation.FindSimulation(worldPosition, sample.Simulation);

					if (simulation != null)
					{
						sample.SetPendingOverlap(simulation.GetOverlapXZ(worldPosition, sample.Radius));

						FlowHelper.BeginActive(FlowReader.AddedBuffer);
							FlowReader.Sample(simulation, worldPosition, sample);
						FlowHelper.EndActive();
					}
					else
					{
						sample.Clear();
					}
				}
			}

			FlowReader.Update();
		}
	}
}

#if UNITY_EDITOR
namespace FLOW
{
	using UnityEditor;
	using TARGET = FlowManager;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class FlowManager_Editor : FlowEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("updateMode", "This allows you to control how often this simulation updates.\n\nEveryFrame = All buffers will be updated every FixedUpdate.\n\nEveryOtherFrame = All buffers will be updated every two FixedUpdate calls.");
		}
	}
}
#endif