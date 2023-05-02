using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static GrpcBase.SolarPanelMessages.Types;

public class TopUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject _powerLabel;

    private async void Start()
    {
        SolarPanelHandler.Instance.OnSolarPanelPlaced.AddListener(new UnityAction(async () => await UpdateUI()));                
        await UpdateUI();
    }

    public async Task UpdateUI()
    {
        var panelInfos = await SolarPanelHandler.Instance.GetInfo();
        var totalPower = panelInfos.Sum(p => p.CurrentPower);
        _powerLabel.GetComponent<TextMeshProUGUI>().text = $"Tot. power: {(int) totalPower}W";
    } 
}
