using GrpcBase;
using GrpcClients;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ChangeImage : MonoBehaviour
{
    public UnityEngine.UI.Image original;

    private GrpcBase.Image.ImageClient _imageClient = Clients.Instance.Image;

    public void NewImageLast24hours(){
        LoadImage(new()
        {
            ImagePath = "last24h",
            Width = 600,
            Height = 420
        });
    }

    public void NewImageLastWeek(){
        LoadImage(new()
        {
            ImagePath = "lastweek",
            Width = 600,
            Height = 420
        });
    }

    public void NewImageLastMonth(){
        LoadImage(new()
        {
            ImagePath = "lastmonth",
            Width = 600,
            Height = 420
        });
    }

    public void NewImageLastYear(){
        LoadImage(new()
        {
            ImagePath = "lastyear",
            Width = 600,
            Height = 420
        });
    }

    private void LoadImage(ImageMessages.Types.ImageRequest request)
    {
        Debug.Log(request.ImagePath);
        var imageData = _imageClient.GetImage(request);
        var texture = ConvertToTexture2D(imageData);
        var startSprite = Sprite.Create(texture, new Rect(0, 0, imageData.Width, imageData.Height), new Vector2(0, 0), 100.0f);

        var image = gameObject.GetComponent<UnityEngine.UI.Image>();
        image.color = UnityEngine.Color.white;
        original.sprite = startSprite;
    }

    private Texture2D ConvertToTexture2D(ImageMessages.Types.ImageData imageData)
    {
        var texture = new Texture2D(imageData.Width, imageData.Height);
        var byteArray = imageData.Data.ToByteArray();
        texture.LoadImage(byteArray);
        return texture;
    }
}
