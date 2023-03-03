import grpc
import logging
import base_pb2_grpc
from concurrent import futures
from netatmo import NetatmoServicer


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    base_pb2_grpc.add_NetatmoServicer_to_server(
        NetatmoServicer(), server)
    base_pb2_grpc.add_ImageServicer_to_server(
        base_pb2_grpc.ImageServicer(), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    server.wait_for_termination()


if __name__ == '__main__':
    logging.basicConfig()
    serve()
