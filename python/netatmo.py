from configparser import ConfigParser
import lnetatmo as nt
import base_pb2_grpc
from base_pb2 import NetatmoMessages as T


def get_netatmo_data(config):
    client_id = config['client_id']
    client_secret = config['client_secret']
    username = config['username']
    password = config['password']

    authorization = nt.ClientAuth(client_id, client_secret, username, password)
    weatherData = nt.WeatherStationData(authorization)

    return weatherData.lastData()


class NetatmoServicer(base_pb2_grpc.NetatmoServicer):

    def __init__(self) -> None:
        parser = ConfigParser()
        parser.read('.env')
        self.config = parser['NETATMO']

    def GetData(self, request, context):
        data = get_netatmo_data(self.config)
        indoor = data['G3 Indoor']
        outdoor = data['G3 Outdoor']
        return T.NetatmoData(
            indoor=T.IndoorData(
                temperature=indoor['Temperature'],
                CO2=indoor['CO2'],
                humidity=indoor['Humidity'],
                noise=indoor['Noise'],
                pressure=indoor['Pressure']),
            outdoor=T.OutdoorData(
                temperature=outdoor['Temperature'],
                humidity=outdoor['Humidity']))
