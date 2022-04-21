using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class Sample : MonoBehaviour
{

    [SerializeField] private AROcclusionManager _arOcclusionManager;
    [SerializeField] private RawImage _rawImage;
    [SerializeField] private RawImage _ColorMapLandscape;
    [SerializeField] private RawImage _ColorMapPortrait;
    [SerializeField] private RawImage _PositionMapLandscape;
    [SerializeField] private RawImage _PositionMapPortrait;

    private void Update()
    {
        _rawImage.texture = _arOcclusionManager.humanDepthTexture;




    }
}