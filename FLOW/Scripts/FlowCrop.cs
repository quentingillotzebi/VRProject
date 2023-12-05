using UnityEngine;

namespace FLOW
{
	/// <summary>This class allows you to crop part of a RenderTexture into a temporary RenderTexture.</summary>
    public static class FlowCrop
    {
        private static Material cachedMaterial;

        public static RenderTexture CropToTemporary(RenderTexture source, RectInt rect)
        {
            if (cachedMaterial == null)
			{
				cachedMaterial = new Material(Resources.Load<Shader>("FLOW/Crop"));
			}

			var offsetX = rect.x      / (float)source.width;
			var offsetY = rect.y      / (float)source.height;
			var scaleX  = rect.width  / (float)source.width;
			var scaleY  = rect.height / (float)source.height;

            cachedMaterial.SetVector("_CropOffset", new Vector2(offsetX, offsetY));
			cachedMaterial.SetVector("_CropScale", new Vector2(scaleX, scaleY));
			cachedMaterial.SetTexture("_CropTexture", source);
			cachedMaterial.SetVector("_CropSize", new Vector2(source.width, source.height));

			var descriptior = source.descriptor;

			descriptior.width  = rect.width;
			descriptior.height = rect.height;

			var tempBuffer = RenderTexture.GetTemporary(descriptior);

			FlowHelper.BeginActive(tempBuffer);
				FlowHelper.Draw(cachedMaterial, 0);
			FlowHelper.EndActive();

			return tempBuffer;
        }
    }
}