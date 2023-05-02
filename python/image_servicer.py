import base_pb2_grpc
import io
from base_pb2 import ImageMessages as TImage, TibberMessages as TTibber
from PIL import Image
from tibber_plot import tibber_realtime
import asyncio
import os
import random


def load_image(file_name: str):
    return Image.open(f'img/{file_name}')


def pil_to_image_data(img: Image) -> TImage.ImageData:
    byteIO = io.BytesIO()
    img.save(byteIO, img.format)
    byteArr = byteIO.getvalue()
    w, h = img.size
    return TImage.ImageData(data=byteArr,
                            format=img.format,
                            width=w,
                            height=h)


class ImageServicer(base_pb2_grpc.ImageServicer):

    def GetImage(self, request: TImage.ImageRequest, context):
        img = load_image(request.imagePath)
        return pil_to_image_data(img)

    def GetTibberImage(self, request: TTibber.Request, context):
        img = tibber_realtime(time_resolution=request.timeResolution,
                              time_units=request.timeUnits)
        return TTibber.Response(image=pil_to_image_data(img))

    async def GetKongefamilieImage(self, request, context):
        folder_path = './kongefamilien'
        while True:
            img_files = [os.path.join(folder_path, f) for f in os.listdir(
                folder_path) if f.endswith(('.png', '.jpg', '.jpeg'))]
            rand_img = random.choice(img_files)
            img = Image.open(rand_img)
            yield pil_to_image_data(img)
            await asyncio.sleep(5)
