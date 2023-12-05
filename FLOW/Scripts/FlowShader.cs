using UnityEngine;

namespace FLOW
{
    /// <summary>This class caches all shader property names to their property index.</summary>
    public static class FlowShader
    {
        public static int _BufferPixel;
        public static int _BufferSize;
        public static int _SamplePixel;
        public static int _FlowResolution;
        public static int _FlowDelta;
        public static int _FlowSpeed;
        public static int _FlowDataA;
        public static int _FlowDataB;
        public static int _FlowDataC;
        public static int _FlowDataD;
        public static int _FlowDataE;
        public static int _FlowDataF;
        public static int _FlowMatrix;
        public static int _FlowCoordU000;
        public static int _FlowCoord0V00;
        public static int _FlowCoordUV00;
        public static int _FlowCountXZ;
        public static int _FlowSeparationXZ;

        public static int _PartCoordUV;
        public static int _PartCountXY;
        public static int _PartDataA;
        public static int _PartDataB;
        public static int _PartDataC;
        public static int _PartDataD;
        public static int _PartDataE;
        public static int _PartDataF;
        public static int _PartDrag;
        public static int _PartGravity;

        public static int _FlowTableDepth;
        public static int _FlowDryRate;
        public static int _FlowFoamClearRate;
        public static int _FlowCameraHeight;
        public static int _FlowSimulationHeight;
        public static int _FlowGroundOffset;
        public static int _FlowSurfaceOffset;

        public static int _ModifierMatrix;
        public static int _ModifierInverse;
        public static int _ModifierBuffer;
        public static int _ModifierStrength;
        public static int _ModifierAngle;
        public static int _ModifierNormal;
        public static int _ModifierShape;
        public static int _ModifierChannel;
        public static int _ModifierRGBA;
        public static int _ModifierChannels;
        public static int _ModifierESMV;
        public static int _ModifierF123;


        static FlowShader()
		{
            _BufferPixel = Shader.PropertyToID("_BufferPixel");
            _BufferSize = Shader.PropertyToID("_BufferSize");
            _SamplePixel = Shader.PropertyToID("_SamplePixel");
            _FlowResolution = Shader.PropertyToID("_FlowResolution");
            _FlowDelta = Shader.PropertyToID("_FlowDelta");
            _FlowSpeed = Shader.PropertyToID("_FlowSpeed");
            _FlowDataA = Shader.PropertyToID("_FlowDataA");
            _FlowDataB = Shader.PropertyToID("_FlowDataB");
            _FlowDataC = Shader.PropertyToID("_FlowDataC");
            _FlowDataD = Shader.PropertyToID("_FlowDataD");
            _FlowDataE = Shader.PropertyToID("_FlowDataE");
            _FlowDataF = Shader.PropertyToID("_FlowDataF");
            _FlowMatrix = Shader.PropertyToID("_FlowMatrix");
            _FlowCoordU000 = Shader.PropertyToID("_FlowCoordU000");
            _FlowCoord0V00 = Shader.PropertyToID("_FlowCoord0V00");
            _FlowCoordUV00 = Shader.PropertyToID("_FlowCoordUV00");
            _FlowCountXZ = Shader.PropertyToID("_FlowCountXZ");
            _FlowSeparationXZ = Shader.PropertyToID("_FlowSeparationXZ");

            _PartCoordUV = Shader.PropertyToID("_PartCoordUV");
            _PartCountXY = Shader.PropertyToID("_PartCountXY");
            _PartDataA = Shader.PropertyToID("_PartDataA");
            _PartDataB = Shader.PropertyToID("_PartDataB");
            _PartDataC = Shader.PropertyToID("_PartDataC");
            _PartDataD = Shader.PropertyToID("_PartDataD");
            _PartDataE = Shader.PropertyToID("_PartDataE");
            _PartDataF = Shader.PropertyToID("_PartDataF");
            _PartDrag = Shader.PropertyToID("_PartDrag");
            _PartGravity = Shader.PropertyToID("_PartGravity");

            _FlowTableDepth = Shader.PropertyToID("_FlowTableDepth");
            _FlowDryRate = Shader.PropertyToID("_FlowDryRate");
            _FlowFoamClearRate = Shader.PropertyToID("_FlowFoamClearRate");
            _FlowSimulationHeight = Shader.PropertyToID("_FlowSimulationHeight");
            _FlowCameraHeight = Shader.PropertyToID("_FlowCameraHeight");
            _FlowGroundOffset = Shader.PropertyToID("_FlowGroundOffset");
            _FlowSurfaceOffset = Shader.PropertyToID("_FlowSurfaceOffset");

            _ModifierMatrix = Shader.PropertyToID("_ModifierMatrix");
            _ModifierInverse = Shader.PropertyToID("_ModifierInverse");
            _ModifierBuffer = Shader.PropertyToID("_ModifierBuffer");
            _ModifierStrength = Shader.PropertyToID("_ModifierStrength");
            _ModifierAngle = Shader.PropertyToID("_ModifierAngle");
            _ModifierNormal = Shader.PropertyToID("_ModifierNormal");
            _ModifierShape = Shader.PropertyToID("_ModifierShape");
            _ModifierChannel = Shader.PropertyToID("_ModifierChannel");
            _ModifierRGBA = Shader.PropertyToID("_ModifierRGBA");
            _ModifierChannels = Shader.PropertyToID("_ModifierChannels");
            _ModifierESMV = Shader.PropertyToID("_ModifierESMV");
            _ModifierF123 = Shader.PropertyToID("_ModifierF123");
        }
    }
}