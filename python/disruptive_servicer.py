import base_pb2_grpc
from base_pb2 import DisruptiveMessages as T
from base_pb2 import ImageMessages as TImage
from image import pil_to_image_data
import asyncio
import grpc
from test import heatmap


class DisruptiveServicer(base_pb2_grpc.DisruptiveServicer):

    async def GetTemperatureStream(self, request, context: grpc.RpcContext):
        i = 0
        while True:
            yield T.Response(sensorName='moren din')
            await asyncio.sleep(2)
            i += 1

    def GetHeatmapImage(self, request: T.HeatmapRequest, context):
        img = heatmap(request.floorNumber)
        return TImage.ImageData(pil_to_image_data(img))
