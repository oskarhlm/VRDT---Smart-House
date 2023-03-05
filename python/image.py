import base_pb2_grpc
import io
from base_pb2 import ImageMessages as T
from PIL import Image


def load_image(file_name: str):
    return Image.open(f'img/{file_name}')


class ImageServicer(base_pb2_grpc.ImageServicer):

    def GetImage(self, request: T.ImageRequest, context):
        byteIO = io.BytesIO()
        img = load_image(request.imagePath)
        img.save(byteIO, img.format)
        byteArr = byteIO.getvalue()
        w, h = img.size
        return T.ImageData(
            data=byteArr,
            format=img.format,
            width=w,
            height=h
        )
