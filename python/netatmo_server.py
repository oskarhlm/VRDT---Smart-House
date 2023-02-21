from concurrent import futures
from configparser import ConfigParser

import grpc
import logging
import lnetatmo as nt
import netatmo_pb2
import netatmo_pb2_grpc


def get_netatmo_data():
    parser = ConfigParser()
    parser.read('.env')
    config = parser['NETATMO']

    client_id = config['client_id']
    client_secret = config['client_secret']
    username = config['username']
    password = config['password']

    authorization = nt.ClientAuth(client_id, client_secret, username, password)
    weatherData = nt.WeatherStationData(authorization)

    return weatherData.lastData()


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
