using UnityEngine;

namespace FLOW
{
	/// <summary>This class allows you to replace part of a RenderTexture with another Texture.</summary>
	public static class FlowReplace
	{
		private static Material cachedMaterial;

		public static void Replace(RenderTexture renderTexture, Texture replaceTexture, Vector4 replaceValues)
		{
			var renderRect = new RectInt(0, 0, renderTexture.width, renderTexture.height);

			Replace(renderTexture, renderRect, replaceTexture, replaceValues);
		}

		public static void Replace(RenderTexture renderTexture, RectInt renderRect, Texture replaceTexture, Vector4 replaceValues)
		{
			if (cachedMaterial == null)
			{
				cachedMaterial = new Material(Resources.Load<Shader>("FLOW/Replace"));
			}

			if (replaceTexture == null)
			{
				replaceTexture = Texture2D.whiteTexture;
			}

			var offsetX = renderRect.x      / (float)renderTexture.width;
			var offsetY = renderRect.y      / (float)renderTexture.height;
			var scaleX  = renderRect.width  / (float)renderTexture.width;
			var scaleY  = renderRect.height / (float)renderTexture.height;

			cachedMaterial.SetVector("_ReplaceOffset", new Vector2(offsetX, offsetY));
			cachedMaterial.SetVector("_ReplaceScale", new Vector2(scaleX, scaleY));
			cachedMaterial.SetTexture("_ReplaceTexture", replaceTexture);
			cachedMaterial.SetVector("_ReplaceValues", replaceValues);
			cachedMaterial.SetVector("_ReplaceSize", new Vector2(replaceTexture.width, replaceTexture.height));

			FlowHelper.BeginActive(renderTexture);
				FlowHelper.Draw(cachedMaterial, 0);
			FlowHelper.EndActive();
		}
	}
}