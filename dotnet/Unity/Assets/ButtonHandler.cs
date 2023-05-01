using GrpcBase;
using GrpcClients;
using UnityEngine;
using static GrpcBase.ImageMessages.Types;
using static GrpcBase.TibberMessages.Types;

public class ButtonHandler : MonoBehaviour
{
    private Image.ImageClient _imageClient = Clients.Instance.Image;

    public void GetImage()
    {
        GameObject numInput = transform.parent.Find("InputField").gameObject;
        GameObject dropdown = transform.parent.Find("Dropdown").gameObject;

        var timeUnits = numInput.GetComponent<EC_NumInput>().GetNumInput();
        var timeResolution = dropdown.GetComponent<EC_dropdown>().GetDDVal();

        var imageData = _imageClient.GetTibberImage(new()
        {
            TimeResolution = timeResolution,
            TimeUnits = timeUnits
        }).Image;

        var texture = ConvertToTexture2D(imageData);
        var imageSprite = Sprite.Create(texture, new Rect(0, 0, imageData.Width, imageData.Height), new Vector2(0, 0), 100.0f);
        transform.parent.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = imageSprite;
    }

    private Texture2D ConvertToTexture2D(ImageData imageData)
    {
        var texture = new Texture2D(imageData.Width, imageData.Height);
        var byteArray = imageData.Data.ToByteArray();
        texture.LoadImage(byteArray);
        return texture;
    }
}
