from concurrent import futures
from netatmo_client import get_netatmo_data

import grpc
import netatmo_pb2
import netatmo_pb2_grpc
import logging


class NetatmoServicer(netatmo_pb2_grpc.NetatmoServicer):

    def GetData(self, request, context):
        data = get_netatmo_data()
        indoor = data['G3 Indoor']
        outdoor = data['G3 Outdoor']
        return netatmo_pb2.NetatmoData(
            indoor=netatmo_pb2.IndoorData(
                temperature=indoor['Temperature'],
                CO2=indoor['CO2'],
                humidity=indoor['Humidity'],
                noise=indoor['Noise'],
                pressure=indoor['Pressure']),
            outdoor=netatmo_pb2.OutdoorData(
                temperature=outdoor['Temperature'],
                humidity=outdoor['Humidity']))


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    netatmo_pb2_grpc.add_NetatmoServicer_to_server(
        NetatmoServicer(), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    server.wait_for_termination()


if __name__ == '__main__':
    logging.basicConfig()
    serve()
