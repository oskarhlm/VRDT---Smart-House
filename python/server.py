import grpc
import logging
import base_pb2_grpc
from concurrent import futures
from netatmo import NetatmoServicer
from image import ImageServicer
from disruptive_servicer import DisruptiveServicer
from solarpanel_servicer import SolarPanelServicer
import asyncio
import signal


class Server:
    def __init__(self):
        self.server = grpc.aio.server(
            futures.ThreadPoolExecutor(max_workers=10))
        base_pb2_grpc.add_NetatmoServicer_to_server(
            NetatmoServicer(), self.server)
        base_pb2_grpc.add_ImageServicer_to_server(
            ImageServicer(), self.server)
        base_pb2_grpc.add_DisruptiveServicer_to_server(
            DisruptiveServicer(), self.server)
        base_pb2_grpc.add_SolarPanelServicer_to_server(
            SolarPanelServicer(), self.server)
        self.server.add_insecure_port('[::]:50051')

    async def start(self):
        await self.server.start()
        await self.server.wait_for_termination()

    async def stop(self):
        self.server.stop(None)


if __name__ == '__main__':
    logging.basicConfig()

    # Create the server object
    server = Server()

    # Register the signal handler
    loop = asyncio.get_event_loop()
    loop.add_signal_handler(
        signal.SIGINT, lambda: asyncio.create_task(server.stop()))

    # Start the server
    asyncio.run(server.start())
