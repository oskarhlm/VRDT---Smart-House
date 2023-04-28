using GrpcClients;
using System.Collections;
using UnityEngine;

public class SolarPanel : MonoBehaviour
{
    [SerializeField] private int infoFetchFrequencyInSeconds = 5;
    [SerializeField] private bool shouldFetch = true;

    private GrpcBase.SolarPanel.SolarPanelClient _client = Clients.Instance.SolarPanel;
    private Transform _panelMesh { get { return transform.Find("Panel_50"); } }

    private void Start()
    {
        //StartCoroutine(FetchInfoWithFrequency());
    }

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

    //private IEnumerator FetchInfoWithFrequency()
    //{
    //    while (shouldFetch)
    //    {
    //        var info = GetInfo();
    //        Debug.Log(info);
    //        yield return new WaitForSeconds(infoFetchFrequencyInSeconds);
    //    }
    //}


    //private GrpcBase.SolarPanelMessages.Types.PanelInfoResponse GetInfo()
    //{
    //    var (width, height) = GetWidthAndHeight();
    //    return _client.GetSolarPanelInfo(new()
    //    {
    //        PanelName = "SPR",
    //        PanelWidth = width,
    //        PanelHeight = height,
    //        Tilt = _panelMesh.eulerAngles.y,
    //        Azimuth = _panelMesh.eulerAngles.z,
    //        Datetime = new() { Seconds = TimeManager.Instance.Date.Second }
    //    });
    //}
}
