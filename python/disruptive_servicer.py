import base_pb2_grpc
from base_pb2 import DisruptiveMessages as T
from time import sleep


class DisruptiveServicer(base_pb2_grpc.DisruptiveServicer):
    def __init__(self) -> None:
        self.count = 0

    async def GetTemperatureStream(self, request_iterator, context):
        while True:
            yield T.Response(sensorName=f'moren din {self.count}')
            self.count += 1
            sleep(5)
