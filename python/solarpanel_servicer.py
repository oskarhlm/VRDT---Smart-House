from base_pb2 import SolarPanelMessages as T
from base_pb2_grpc import SolarPanelServicer
from typing import List
from solarpanels_refactored import SPRMax400, get_power_stats, get_noon_yesterday


class SolarPanelServicer(SolarPanelServicer):

    def GetSolarPanelInfo(self, request_iterator: List[T.PanelInfoRequest], context):
        for request in request_iterator:
            panel = SPRMax400(tilt=request.tilt, azimuth=request.azimuth)
            yield get_power_stats(panel, time=get_noon_yesterday())
            # yield get_power_stats(panel, time=request.datetime.ToDatetime())
