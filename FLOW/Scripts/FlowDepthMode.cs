using UnityEngine;

namespace FLOW
{
	[ExecuteAlways]
	[RequireComponent(typeof(Camera))]
	public class FlowDepthMode : MonoBehaviour
	{
		public DepthTextureMode Mode { set { mode = value; } get { return mode; } } [SerializeField] private DepthTextureMode mode;

		protected virtual void Update()
		{
			GetComponent<Camera>().depthTextureMode = mode;
		}
	}
}