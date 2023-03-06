using GrpcBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Windows.Speech;
using GrpcClients;

public class TvImage : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    private GrpcBase.Image.ImageClient _imageClient = Clients.Instance.Image;

    // Start is called before the first frame update
    void Start()
    {
        actions.Add("NTNU", Ntnu);
        actions.Add("temperature graph", TemperatureGraph);
        actions.Add("up", () => Debug.Log("Up"));

        foreach (var d in Microphone.devices)
        {
            Debug.Log(d);
        }

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Ntnu() =>
        LoadImage(new()
        {
            ImagePath = "NTNU.jpg",
            Width = 600,
            Height = 420
        });

    private void TemperatureGraph() =>
        LoadImage(new()
        {
            ImagePath = "temperature_graph.png",
            Width = 600,
            Height = 420
        });

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    private void LoadImage(ImageMessages.Types.ImageRequest request)
    {
        var imageData = _imageClient.GetImage(request);
        var texture = ConvertToTexture2D(imageData);
        var startSprite = Sprite.Create(texture, new Rect(0, 0, imageData.Width, imageData.Height), new Vector2(0, 0), 100.0f);

        var image = gameObject.GetComponent<UnityEngine.UI.Image>();
        image.color = UnityEngine.Color.white;
        image.sprite = startSprite;
    }

    private Texture2D ConvertToTexture2D(ImageMessages.Types.ImageData imageData)
    {
        var texture = new Texture2D(imageData.Width, imageData.Height);
        var byteArray = imageData.Data.ToByteArray();
        texture.LoadImage(byteArray);
        return texture;
    }
}
