using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using static GrpcBase.ImageMessages.Types;

public class HeatmapHandler : MonoBehaviour
{
    private GrpcBase.Disruptive.DisruptiveClient _client = GrpcClients.Clients.Instance.Disruptive;
    private Transform _canvas;

    void Start()
    {
        _canvas = transform.parent;
    }

    public void ChangePanelImage(int floor)
    {
        var imageData = GetHeatmapImageData(floor);
        SetImage(imageData);
    }

    private ImageData GetHeatmapImageData(int floor)
    {
        return _client.GetHeatmapImage(new()
        {
            FloorNumber = floor
        });
    }
    
    private void SetImage(ImageData imageData)
    {
        var imageComponent = transform.Find("Image").GetComponent<Image>();
        imageComponent.color = Color.white;
        imageComponent.sprite = ImageUtils.CreateSprite(imageData);
    } 
}
