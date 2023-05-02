using Grpc.Core;
using GrpcClients;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Utils;
using static GrpcBase.SolarPanelMessages.Types;

public class SolarPanelHandler : Singleton<SolarPanelHandler> 
{
    public UnityEvent OnSolarPanelPlaced;

    private List<SolarPanel> _solarPanels = new();

    [SerializeField] private int infoFetchFrequencyInSeconds = 5;
    [SerializeField] private bool shouldFetch = true;

    private GrpcBase.SolarPanel.SolarPanelClient _client = Clients.Instance.SolarPanel;

    private void Start()
    {
        OnSolarPanelPlaced.AddListener(new UnityAction(async () => await GetInfo()));
    }

    public void Add(SolarPanel panel)
    {
        _solarPanels.Add(panel);
    }

    public async Task<Collection<PanelInfoResponse>> GetInfo()
    {
        var call = _client.GetSolarPanelInfo();
        var panelInfos = new Collection<PanelInfoResponse>();
        foreach (var panel in _solarPanels)
        {
            var info = await panel.GetInfo(call);
            panelInfos.Add(info);
        }
        var totalPower = panelInfos.Sum(p => p.CurrentPower);
        Debug.Log($"Tot. power: {totalPower}");
        call.Dispose();
        return panelInfos;
    }
}
