using GrpcBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GrpcBase.Image;
using UnityEngine;
using static GrpcBase.ImageMessages.Types;

namespace Assets
{
    public static class ImageUtils
    {
        private static Texture2D ConvertToTexture2D(ImageData imageData)
        {
            var texture = new Texture2D(imageData.Width, imageData.Height);
            var byteArray = imageData.Data.ToByteArray();
            texture.LoadImage(byteArray);
            return texture;
        }

        public static Sprite CreateSprite(ImageData imageData)
        {
            var texture = ConvertToTexture2D(imageData);
            return Sprite.Create(texture, new Rect(0, 0, imageData.Width, imageData.Height), new Vector2(0, 0), 100.0f);

        }
    }
}
