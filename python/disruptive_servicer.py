import base_pb2_grpc
from base_pb2 import DisruptiveMessages as T
import asyncio
import grpc


class DisruptiveServicer(base_pb2_grpc.DisruptiveServicer):

    async def GetTemperatureStream(self, request, context: grpc.RpcContext):
        i = 0
        while True:
            yield T.Response(sensorName=f'moren din {i}')
            await asyncio.sleep(2)
            i += 1
