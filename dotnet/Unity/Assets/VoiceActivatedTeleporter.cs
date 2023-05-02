using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class VoiceActivatedTeleporter : MonoBehaviour
{
    private KeywordRecognizer _keywordRecognizer;
    private Dictionary<string, Action> _actions = new Dictionary<string, Action>();
    private GameObject _playerOrigin;


    private readonly (Vector3, Quaternion) ROOF = 
        (new Vector3(13.1300001f, 26.2999992f, 1.94000006f), new Quaternion(0, -0.707106829f, 0, 0.707106829f));
    private readonly (Vector3, Quaternion) LIVING_ROOM 
        = (new Vector3(7.90995836f, 22f, -0.281673968f), new Quaternion(0, 0.197483301f, 0, 0.980306327f));

    void Start()
    {
         _playerOrigin = GameObject.Find("XR Origin");

        _actions.Add("teleport roof", () => Teleport(ROOF, gravity: false));
        _actions.Add("teleport livingroom", () => Teleport(LIVING_ROOM));

        _keywordRecognizer = new KeywordRecognizer(_actions.Keys.ToArray());
        _keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        _keywordRecognizer.Start();
    }

    private void Teleport((Vector3, Quaternion) spawn, bool gravity = true)
    {
        var moveProvider = _playerOrigin.GetComponent<DynamicMoveProvider>();
        moveProvider.useGravity = gravity;
        moveProvider.enableFly = gravity;
        _playerOrigin.transform.SetPositionAndRotation(spawn.Item1, spawn.Item2);
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log($"Voice command: '{speech.text}'");
        _actions[speech.text].Invoke();
    }
}
