//using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;

//[VolumeComponentMenuForRenderPipeline("Custom/PSX Effect", typeof(UniversalRenderPipeline))]
//public class PSXVolumeComponent : VolumeComponent, IPostProcessComponent
//{
//    public FloatParameter pixelation = new FloatParameter(4.0f);
//    public FloatParameter colorDepth = new FloatParameter(255.0f);
//    public FloatParameter noiseIntensity = new FloatParameter(0.15f);
//    public FloatParameter vignetteIntensity = new FloatParameter(1.0f);
//    public FloatParameter fisheyeStrength = new FloatParameter(0.0f);

//    public bool IsActive()
//    {
//        return pixelation.value > 1.0f ||
//               colorDepth.value < 255.0f ||
//               noiseIntensity.value > 0.0f ||
//               vignetteIntensity.value != 1.0f ||
//               fisheyeStrength.value > 0.0f;
//    }

//    public bool IsTileCompatible() => false;
//}