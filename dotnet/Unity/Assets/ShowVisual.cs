using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System;
using System.Linq;

public class ShowVisual : MonoBehaviour
{
    [SerializeField] private GameObject EnergyPanel;
    [SerializeField] private GameObject HeatmapPanel;

    private GameObject _currentPanel;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    private void Start()
    {
        actions.Add("toggle menu", TogglePanel);
        actions.Add("energy", () => SetCurrentPanel(EnergyPanel));
        actions.Add("heatmap", () => SetCurrentPanel(HeatmapPanel));

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
    }

    public void TogglePanel()
    {
        if (_currentPanel != null)
        {
            bool isActive = _currentPanel.activeSelf;
            _currentPanel.SetActive(!isActive);
        }
    }

    private void SetCurrentPanel(GameObject panel)
    {
        if (_currentPanel != null) _currentPanel.SetActive(false);
        _currentPanel = panel;
        _currentPanel.SetActive(true);        
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }
}
