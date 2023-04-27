using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Utils;

public class SolarPanelHandler : Singleton<SolarPanel> 
{
    private Collection<SolarPanel> _solarPanels = new();

    private void GetPanelInfo()
    {
        var infos = _solarPanels.ToList().Select(sp =>
        {
            "hei": "hei";
        });
    }

}
