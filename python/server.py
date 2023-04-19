import grpc
import logging
import base_pb2_grpc
from concurrent import futures
from netatmo import NetatmoServicer
from image import ImageServicer
from disruptive_servicer import DisruptiveServicer
import asyncio


async def serve():
    server = grpc.aio.server(futures.ThreadPoolExecutor(max_workers=10))
    base_pb2_grpc.add_NetatmoServicer_to_server(
        NetatmoServicer(), server)
    base_pb2_grpc.add_ImageServicer_to_server(
        ImageServicer(), server)
    base_pb2_grpc.add_DisruptiveServicer_to_server(
        DisruptiveServicer(), server)
    server.add_insecure_port('[::]:50051')
    await server.start()
    await server.wait_for_termination()


if __name__ == '__main__':
    logging.basicConfig()
    asyncio.run(serve())
