using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System;
using System.Linq;
using Assets;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class ShowVisual : MonoBehaviour
{
    [SerializeField] private GameObject EnergyPanel;
    [SerializeField] private GameObject HeatmapPanel;
    [SerializeField] private GameObject SolarStatsPanel;

    private GameObject _currentPanel;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    private void Start()
    {

        actions.Add("toggle menu", TogglePanel);
        actions.Add("energy", () =>
        {
            SetCurrentPanel(EnergyPanel);
            transform.GetComponent<TrackedDeviceGraphicRaycaster>().enabled = true;
        });
        actions.Add("heatmap basement", () => SetHeatmapAsCurrentPanel(1));
        actions.Add("heatmap first floor", () => SetHeatmapAsCurrentPanel(2));
        actions.Add("heatmap second floor", () => SetHeatmapAsCurrentPanel(3));
        actions.Add("solar", () =>
        {
            SetCurrentPanel(SolarStatsPanel);
            transform.GetComponent<TrackedDeviceGraphicRaycaster>().enabled = false;
        });

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();

        actions["solar"].Invoke();
    }

    public void TogglePanel()
    {
        bool isActive = _currentPanel.activeSelf;
        _currentPanel.SetActive(!isActive);
    }

    private void SetCurrentPanel(GameObject panel)
    {
        if (_currentPanel != null) _currentPanel.SetActive(false);
        _currentPanel = panel;
        _currentPanel.SetActive(true);        
    }

    private void SetHeatmapAsCurrentPanel(int floor)
    {
        SetCurrentPanel(HeatmapPanel);
        var heatmapHandler = HeatmapPanel.GetComponent<HeatmapHandler>();
        heatmapHandler.ChangePanelImage(floor);
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log($"Voice command: '{speech.text}'");
        actions[speech.text].Invoke();
    }
}
