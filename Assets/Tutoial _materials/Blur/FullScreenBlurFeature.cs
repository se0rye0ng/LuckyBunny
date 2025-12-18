// FullScreenBlurFeature.cs (Unity 2023+ 호환)
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class FullScreenBlurFeature : ScriptableRendererFeature
{
    class FullScreenBlurPass : ScriptableRenderPass
    {
        private Material _blurMaterial;
        private RTHandle _tempTexture;
        private RTHandle _originalTexture;

        // 생성자: Material과 Pass의 이름 설정
        public FullScreenBlurPass(Material material)
        {
            _blurMaterial = material;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        // 카메라 설정 시 소스 텍스처를 지정
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0; // Color only

            // RTHandle 생성
            RenderingUtils.ReAllocateHandleIfNeeded(ref _tempTexture, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_TemporaryColorTexture");
            RenderingUtils.ReAllocateHandleIfNeeded(ref _originalTexture, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_OriginalColorTexture");
        }

        // Pass 실행 (Legacy)
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_blurMaterial == null || renderingData.cameraData.isPreviewCamera) return;

            CommandBuffer cmd = CommandBufferPool.Get("Full Screen Blur");

            RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            // === 마우스 좌표 전달 로직 추가 ===
            // 마우스 위치를 0~1 사이의 Screen UV 좌표로 변환
            Vector4 mousePos = Input.mousePosition;
            mousePos.x /= Screen.width;
            mousePos.y /= Screen.height;

            // Material 프로퍼티로 설정
            _blurMaterial.SetVector("_MousePos", mousePos);
            // =================================

            // Step 0: 원본 이미지 저장 (블러 전)
            Blitter.BlitCameraTexture(cmd, source, _originalTexture);

            // Step 1: Horizontal Blur (Pass Index 0)
            Blitter.BlitCameraTexture(cmd, source, _tempTexture, _blurMaterial, 0);

            // Step 2: Vertical Blur + Masking (Pass Index 1)
            // 원본 텍스처를 셰이더에 전달
            _blurMaterial.SetTexture("_OriginalTex", _originalTexture);
            Blitter.BlitCameraTexture(cmd, _tempTexture, source, _blurMaterial, 1);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // RenderGraph 실행 (Unity 2023+)
        // Compatibility Mode를 사용하므로 이 메서드는 호출되지 않음
        // 하지만 경고를 피하기 위해 빈 구현 제공
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            // Compatibility Mode에서는 Execute() 메서드가 대신 사용됨
        }

        // 리소스 정리
        public void Dispose()
        {
            _tempTexture?.Release();
            _originalTexture?.Release();
        }
    }

    // Feature의 필드 정의
    public Material blurMaterial;
    private FullScreenBlurPass _blurPass;

    public override void Create()
    {
        _blurPass = new FullScreenBlurPass(blurMaterial);
    }
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (blurMaterial == null) return;
        renderer.EnqueuePass(_blurPass);
    }

    protected override void Dispose(bool disposing)
    {
        _blurPass?.Dispose();
    }
}