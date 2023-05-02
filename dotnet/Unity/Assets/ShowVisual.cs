using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System;
using System.Linq;
using Assets;

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
        SetCurrentPanel(SolarStatsPanel);

        actions.Add("toggle menu", TogglePanel);
        actions.Add("energy", () => SetCurrentPanel(EnergyPanel));
        actions.Add("heatmap", () => SetCurrentPanel(HeatmapPanel));
        actions.Add("solar", () => SetCurrentPanel(SolarStatsPanel));

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
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

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log($"Voice command: '{speech.text}'");
        actions[speech.text].Invoke();
    }
}
