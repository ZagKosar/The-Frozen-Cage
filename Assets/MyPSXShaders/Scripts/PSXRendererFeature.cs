//using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;

//public class PSXRendererFeature : ScriptableRendererFeature
//{
//    class PSXRenderPass : ScriptableRenderPass
//    {
//        public Material material;
//        public PSXVolumeComponent volumeComponent;
//        private RTHandle source;
//        private RTHandle destination;

//        public void Setup(RTHandle source, RTHandle destination)
//        {
//            this.source = source;
//            this.destination = destination;
//        }

//        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
//        {
//            if (material == null || volumeComponent == null || !volumeComponent.IsActive()) return;

//            // Передаём значения из Volume в шейдер
//            material.SetFloat("_Pixelation", volumeComponent.pixelation.value);
//            material.SetFloat("_ColorDepth", volumeComponent.colorDepth.value);
//            material.SetFloat("_NoiseIntensity", volumeComponent.noiseIntensity.value);
//            material.SetFloat("_VignetteIntensity", volumeComponent.vignetteIntensity.value);
//            material.SetFloat("_FisheyeStrength", volumeComponent.fisheyeStrength.value);

//            CommandBuffer cmd = CommandBufferPool.Get();
//            using (new ProfilingScope(cmd, new ProfilingSampler("PSX Post Process")))
//            {
//                // Современный URP-блит, корректно работает с RTHandle и XR
//                Blitter.BlitCameraTexture(cmd, source, destination, material, 0);
//            }
//            context.ExecuteCommandBuffer(cmd);
//            CommandBufferPool.Release(cmd);
//        }
//    }

//    [SerializeField] private Material material;
//    private PSXRenderPass pass;

//    public override void Create()
//    {
//        pass = new PSXRenderPass
//        {
//            renderPassEvent = RenderPassEvent.AfterRenderingTransparents // Применяется после основной отрисовки
//        };
//    }

//    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
//    {
//        if (material == null) return;

//        var volume = VolumeManager.instance.stack.GetComponent<PSXVolumeComponent>();
//        if (volume == null) return;

//        pass.material = material;
//        pass.volumeComponent = volume;
//        pass.Setup(renderer.cameraColorTargetHandle, renderer.cameraColorTargetHandle);
//        renderer.EnqueuePass(pass);
//    }
//}