using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;

namespace FLOW
{
	public static class FlowHelper
	{
		public const string HelpUrlPrefix = "http://carloswilkes.github.io/Documentation/FLOW#";

		public const string ComponentMenuPrefix = "FLOW/FLOW ";

		public const string GameObjectMenuPrefix = "GameObject/FLOW/";

		private static Stack<RenderTexture> actives = new Stack<RenderTexture>();

		public static void Resize<T>(ref NativeArray<T> array, int size)
			where T : struct
		{
			if (array.IsCreated == false)
			{
				array = new NativeArray<T>(size, Allocator.Persistent);
			}
			else if (array.Length != size)
			{
				array.Dispose();

				array = new NativeArray<T>(size, Allocator.Persistent);
			}
		}

		public static float UniformScale(Vector3 scale)
		{
			return System.Math.Max(System.Math.Max(scale.x, scale.y), scale.z);
		}

		public static float DampenFactor(float speed, float elapsed)
		{
			if (speed < 0.0f)
			{
				return 1.0f;
			}
#if UNITY_EDITOR
			if (Application.isPlaying == false)
			{
				return 1.0f;
			}
#endif
			return 1.0f - Mathf.Pow((float)System.Math.E, -speed * elapsed);
		}

		public static int Mod(int a, int b)
		{
			var m = a % b;

			if (m < 0)
			{
				return m + b;
			}

			return m;
		}

		public static float Mod(float a, float b)
		{
			var m = a % b;

			if (m < 0.0f)
			{
				return m + b;
			}

			return m;
		}

		private static Mesh quadMesh;
		private static bool quadMeshSet;

		public static Mesh GetQuadMesh()
		{
			if (quadMeshSet == false)
			{
				var gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);

				quadMeshSet = true;
				quadMesh    = gameObject.GetComponent<MeshFilter>().sharedMesh;

				Object.DestroyImmediate(gameObject);
			}

			return quadMesh;
		}

		private static Matrix4x4 identityMatrix = Matrix4x4.identity;

		public static void Draw(Material material, int pass)
		{
			if (material.SetPass(pass) == true)
			{
				Graphics.DrawMeshNow(GetQuadMesh(), identityMatrix, 0);
			}
		}

		public static void BeginActive(RenderTexture renderTexture)
		{
			actives.Push(RenderTexture.active);

			RenderTexture.active = renderTexture;
		}

		public static void EndActive()
		{
			RenderTexture.active = actives.Pop();
		}

		public static GameObject CreateGameObject(string name, int layer, Transform parent = null, bool recordUndo = true)
		{
			return CreateGameObject(name, layer, parent, Vector3.zero, Quaternion.identity, Vector3.one, recordUndo);
		}

		public static GameObject CreateGameObject(string name, int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, bool recordUndo = true)
		{
			var gameObject = new GameObject(name);

			gameObject.layer = layer;

			gameObject.transform.SetParent(parent, false);

			gameObject.transform.localPosition = localPosition;
			gameObject.transform.localRotation = localRotation;
			gameObject.transform.localScale    = localScale;

#if UNITY_EDITOR
			if (recordUndo == true)
			{
				UnityEditor.Undo.RegisterCreatedObjectUndo(gameObject, "Create " + name);
			}
#endif

			return gameObject;
		}

#if UNITY_EDITOR
		public static void SelectAndPing(Object o)
		{
			UnityEditor.Selection.activeObject = o;

			UnityEditor.EditorApplication.delayCall += () => UnityEditor.EditorGUIUtility.PingObject(o);
		}
#endif

#if UNITY_EDITOR
		public static Transform GetSelectedParent()
		{
			if (UnityEditor.Selection.activeGameObject != null)
			{
				return UnityEditor.Selection.activeGameObject.transform;
			}

			return null;
		}
#endif
	}
}