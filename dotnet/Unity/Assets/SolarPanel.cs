using Grpc.Core;
using System.Threading.Tasks;
using UnityEngine;
using static GrpcBase.SolarPanelMessages.Types;


public class SolarPanel : MonoBehaviour
{
    [SerializeField] private int infoFetchFrequencyInSeconds = 5;
    [SerializeField] private bool shouldFetch = true;

    private Transform _panelMesh { get { return transform.Find("Panel_50"); } }

    public void SetSnapPlane(BoxCollider collider)
    {
        var snapPlane = _panelMesh.GetComponentInChildren<EdgeSnapping>();
        snapPlane.TargetCollider = collider;
    }

    public (float width, float length) GetWidthAndHeight()
    {
        Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
        Bounds bounds = renderer.bounds;

        float width = bounds.size.x;
        float length = bounds.size.z;
        
        return (width, length);
    }

    public (float tilt, float azimuth) GetTiltAndAzimuth()
    {
        return (_panelMesh.eulerAngles.y, _panelMesh.eulerAngles.z);
    }

    public async Task<PanelInfoResponse> GetInfo(AsyncDuplexStreamingCall<PanelInfoRequest, PanelInfoResponse> call)
    {
        var (width, height) = GetWidthAndHeight();
        var (tilt, azimuth) = GetTiltAndAzimuth();
        await call.RequestStream.WriteAsync(new()
        {
            PanelName = "SPR",
            PanelWidth = width,
            PanelHeight = height,
            Tilt = tilt,
            Azimuth = azimuth,
            Datetime = new() { Seconds = TimeManager.Instance.Time.Second }
        });
        await call.ResponseStream.MoveNext();
        var response = call.ResponseStream.Current;
        //Debug.Log($"Received response: {response}");
        return response;
    }
}
