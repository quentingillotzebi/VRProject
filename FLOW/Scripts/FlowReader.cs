using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;

namespace FLOW
{
	/// <summary>This class allows you to read pixels from RenderTextures asynchronously.</summary>
	public class FlowReader
	{
		public List<Vector2Int> Pixels = new List<Vector2Int>();

		private ISampleHandler handler;

		private static List<FlowReader> addedReaders = new List<FlowReader>();

		private static List<FlowReader> waitingReaders = new List<FlowReader>();

		private static Stack<FlowReader> pool = new Stack<FlowReader>();

		private static RenderTexture addedBuffer;

		private static RenderTexture sampleBuffer;

		private static int index;

		private static Texture2D readBuffer;

		private static Material cachedSampleMaterial;

		private static bool cachedSampleSet;

		private static List<Color> tempColors = new List<Color>();

		private static AsyncGPUReadbackRequest request;

		private static bool requestSent;

		private const int BUFFER_SIZE = 1024;

		public static bool Ready
		{
			get
			{
				return waitingReaders.Count == 0;
			}
		}

		public Vector2Int AllocatePixel()
		{
			var bufferPixel = new Vector2Int(index, 0);

			Pixels.Add(bufferPixel);

			index++;

			return bufferPixel;
		}

		public static void Update()
		{
			// Request new samples?
			if (addedReaders.Count > 0 && waitingReaders.Count == 0)
			{
				waitingReaders.AddRange(addedReaders);

				sampleBuffer = RenderTexture.GetTemporary(addedBuffer.descriptor);

				Graphics.Blit(addedBuffer, sampleBuffer);

				addedReaders.Clear();

				index = 0;

				if (SystemInfo.supportsAsyncGPUReadback == true)
				{
					request     = AsyncGPUReadback.Request(sampleBuffer);
					requestSent = true;
				}
			}

			// Process samples?
			if (waitingReaders.Count > 0)
			{
				var pixels = default(NativeArray<Color>);

				if (requestSent == true)
				{
					if (request.hasError == true)
					{
						pixels = GetPixels(sampleBuffer);
					}
					else
					{
						if (request.done == false)
						{
							return;
						}

						pixels      = request.GetData<Color>();
						requestSent = false;
					}
				}
				else
				{
					pixels = GetPixels(sampleBuffer);
				}

				foreach (var reader in waitingReaders)
				{
					tempColors.Clear();

					foreach (var pixel in reader.Pixels)
					{
						tempColors.Add(pixels[pixel.x + pixel.y * sampleBuffer.width]);
					}

					reader.Complete(tempColors);
				}

				RenderTexture.ReleaseTemporary(sampleBuffer);

				sampleBuffer = null;

				waitingReaders.Clear();
			}
		}

		private static NativeArray<Color> GetPixels(RenderTexture buffer)
		{
			if (readBuffer == null)
			{
				readBuffer = new Texture2D(buffer.width, buffer.height, TextureFormat.RGBAFloat, false, true);
			}

			readBuffer.Reinitialize(buffer.width, buffer.height);

			//if (SystemInfo.graphicsUVStartsAtTop == true)
			//{
			//	y = rt.height - y;
			//}

			FlowHelper.BeginActive(buffer);
				readBuffer.ReadPixels(new Rect(0, 0, BUFFER_SIZE, 1), 0, 0);
			FlowHelper.EndActive();

			readBuffer.Apply();

			return readBuffer.GetRawTextureData<Color>();
		}

		public void Complete(List<Color> value)
		{
			handler.HandleSamples(value);

			Pixels.Clear();

			pool.Push(this);
		}

		/*
		public static void Sample(Texture texture, Vector2Int pixel, ISampleHandler handler)
		{
			if (texture != null && handler != null)
			{
				var bufferPixel = InitStart(handler);
			}
		}
		*/

		public static RenderTexture AddedBuffer
		{
			get
			{
				return addedBuffer;
			}
		}

		public static void Sample(FlowSimulation simulation, Vector3 worldPosition, ISampleHandler handler)
		{
			if (simulation != null && simulation.Activated == true && handler != null)
			{
				var reader = InitStart(handler);

				if (cachedSampleSet == false)
				{
					cachedSampleMaterial = new Material(Resources.Load<Shader>("FLOW/Reader_Sample"));
					cachedSampleSet      = true;
				}

				simulation.SetVariables(cachedSampleMaterial);

				var samplePixel = simulation.GetWorldToPixelMatrix().MultiplyPoint(worldPosition);

				cachedSampleMaterial.SetVector(FlowShader._SamplePixel, (Vector2)samplePixel);
				cachedSampleMaterial.SetVector(FlowShader._BufferSize, new Vector2(addedBuffer.width, addedBuffer.height));

				// VelocityXZ, GroundHeight, WetHeight
				cachedSampleMaterial.SetVector(FlowShader._BufferPixel, (Vector2)reader.AllocatePixel());

				FlowHelper.Draw(cachedSampleMaterial, 0);

				// NormalXYZ, Depth
				cachedSampleMaterial.SetVector(FlowShader._BufferPixel, (Vector2)reader.AllocatePixel());

				FlowHelper.Draw(cachedSampleMaterial, 1);

				// RGBA
				cachedSampleMaterial.SetVector(FlowShader._BufferPixel, (Vector2)reader.AllocatePixel());

				FlowHelper.Draw(cachedSampleMaterial, 2);

				// ESMV
				cachedSampleMaterial.SetVector(FlowShader._BufferPixel, (Vector2)reader.AllocatePixel());

				FlowHelper.Draw(cachedSampleMaterial, 3);

				// F123
				cachedSampleMaterial.SetVector(FlowShader._BufferPixel, (Vector2)reader.AllocatePixel());

				FlowHelper.Draw(cachedSampleMaterial, 4);
			}
		}

		private static FlowReader InitStart(ISampleHandler handler)
		{
			var reader = pool.Count > 0 ? pool.Pop() : new FlowReader();

			if (addedBuffer == null)
			{
				addedBuffer = new RenderTexture(BUFFER_SIZE, 1, 0, RenderTextureFormat.ARGBFloat, 0);
			}

			reader.handler = handler;

			addedReaders.Add(reader);

			return reader;
		}
	}
}