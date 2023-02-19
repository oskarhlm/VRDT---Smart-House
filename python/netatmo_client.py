import lnetatmo as nt
from configparser import ConfigParser

parser = ConfigParser()
parser.read('.env')
config = parser['NETATMO']

client_id = config['client_id']
client_secret = config['client_secret']
username = config['username']
password = config['password']

authorization = nt.ClientAuth(client_id, client_secret, username, password)
weatherData = nt.WeatherStationData(authorization)


def get_netatmo_data():
    return weatherData.lastData()
