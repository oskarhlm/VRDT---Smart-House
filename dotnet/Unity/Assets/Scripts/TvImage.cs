using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using GrpcClients;
using Assets;
using GrpcBase;
using static GrpcBase.Disruptive;
using System.Threading;
using UnityEngine.UIElements;

public class TvImage : MonoBehaviour
{
    private GrpcBase.Image.ImageClient _imageClient = Clients.Instance.Image;

    //private async void Start()
    //{
    //    var image = gameObject.GetComponent<UnityEngine.UI.Image>();

    //    var request = new ImageMessages.Types.KongefamilieRequest();
    //    var call = _imageClient.GetKongefamilieImage(request);
    //    while (await call.ResponseStream.MoveNext(CancellationToken.None))
    //    {
    //        var response = call.ResponseStream.Current;
    //        image.sprite = ImageUtils.CreateSprite(response);
    //    }
    //}

    //private void LoadImage(ImageRequest request)
    //{
    //    var imageData = _imageClient.GetImage(request);
    //    var texture = ImageUtils.ConvertToTexture2D(imageData);
    //    var startSprite = Sprite.Create(texture, new Rect(0, 0, imageData.Width, imageData.Height), new Vector2(0, 0), 100.0f);

    //    var image = gameObject.GetComponent<UnityEngine.UI.Image>();
    //    image.color = UnityEngine.Color.white;
    //    image.sprite = startSprite;
    //}
}
