using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(Camera))]
public class AfterImagePostEffect : MonoBehaviour
{
    [SerializeField] private AROcclusionManager _occlusionManager;
    [SerializeField] private Shader _shader;

    private const int NUM_OF_IMAGES = 10;
    private const int FRAME_OF_INTERVAL = 4;

    private readonly (int, int)[] _humanStencilTextureResolution =
    {
        (256, 192), // Fastest
        (960, 720), // Medium
        (1920, 1440) // Best
    };

    private readonly List<AfterImage> _afterImages = new List<AfterImage>();
    private readonly List<RenderTexture> _cameraFeedBuffers = new List<RenderTexture>();
    private readonly List<RenderTexture> _stencilBuffers = new List<RenderTexture>();

    private Camera _camera;
    private CommandBuffer _commandBuffer;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        for (int i = 0; i < NUM_OF_IMAGES; i++)
        {
            _afterImages.Add(new AfterImage(_camera, new Material(_shader)));
        }

        var resolution = (0, 0);
        switch (_occlusionManager.currentHumanStencilMode)
        {
            case HumanSegmentationStencilMode.Fastest:
                resolution = _humanStencilTextureResolution[0];
                break;
            case HumanSegmentationStencilMode.Medium:
                resolution = _humanStencilTextureResolution[1];
                break;
            case HumanSegmentationStencilMode.Best:
                resolution = _humanStencilTextureResolution[2];
                break;
        }

        for (int i = 0; i < (NUM_OF_IMAGES - 1) * FRAME_OF_INTERVAL + 1; i++)
        {
            _cameraFeedBuffers.Add(new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0));
            _stencilBuffers.Add(new RenderTexture(resolution.Item1, resolution.Item2, 0));
        }

        _commandBuffer = new CommandBuffer();
        _commandBuffer.Blit(null, _cameraFeedBuffers.Last());
        _camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, _commandBuffer);
    }

    private void Update()
    {
        for (int i = 0; i < _cameraFeedBuffers.Count - 1; i++)
        {
            Graphics.Blit(_cameraFeedBuffers[i + 1], _cameraFeedBuffers[i]);
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        var humanStencil = _occlusionManager.humanStencilTexture;
        if (humanStencil)
        {
            // デバイスの向きが変更された時用
            if (_cameraFeedBuffers.Last().width != _camera.pixelWidth)
            {
                ReInitCameraFeedBuffers();
            }

            for (int i = 0; i < _stencilBuffers.Count - 1; i++)
            {
                Graphics.Blit(_stencilBuffers[i + 1], _stencilBuffers[i]);
            }

            Graphics.Blit(humanStencil, _stencilBuffers.Last());

            for (int i = 0; i < _afterImages.Count; i++)
            {
                _afterImages[i].SetMaterialProperty(_stencilBuffers[i * FRAME_OF_INTERVAL]);
            }
        }

        Graphics.Blit(src, dest);
    }

    private void OnGUI()
    {
        if (Event.current.type.Equals(EventType.Repaint))
        {
            for (int i = 0; i < _afterImages.Count; i++)
            {
                _afterImages[i].Draw(_cameraFeedBuffers[i * FRAME_OF_INTERVAL]);
            }
        }
    }

    private void ReInitCameraFeedBuffers()
    {
        _commandBuffer.Clear();
        _camera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, _commandBuffer);
        var total = _cameraFeedBuffers.Count;
        foreach (var cameraFeed in _cameraFeedBuffers)
        {
            cameraFeed.Release();
        }

        _cameraFeedBuffers.Clear();

        for (int i = 0; i < total; i++)
        {
            _cameraFeedBuffers.Add(new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0));
        }

        _camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, _commandBuffer);
    }
}