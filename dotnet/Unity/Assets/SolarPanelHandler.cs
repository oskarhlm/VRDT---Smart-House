using GrpcClients;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Utils;

public class SolarPanelHandler : Singleton<SolarPanel> 
{
    public Collection<SolarPanel> _solarPanels = new();
    private GrpcBase.SolarPanel.SolarPanelClient _client = Clients.Instance.SolarPanel;

    private GrpcBase.SolarPanelMessages.Types.PanelInfoResponse GetPanelInfo(SolarPanel panel)
    {
        var (width, height) = panel.GetWidthAndHeight();
        return _client.GetSolarPanelInfo(new()
        {
            PanelName = "SPR",
            PanelWidth = width,
            PanelHeight = height,
            Tilt = 25,
            Azimuth = 180,
            Datetime = new() { Seconds = TimeManager.Instance.Date.Second }
        });
    }

}
