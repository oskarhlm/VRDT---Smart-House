from google.protobuf import timestamp_pb2 as _timestamp_pb2
from google.protobuf.internal import enum_type_wrapper as _enum_type_wrapper
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Mapping as _Mapping, Optional as _Optional, Union as _Union

DESCRIPTOR: _descriptor.FileDescriptor

class DisruptiveMessages(_message.Message):
    __slots__ = []
    class HeatmapRequest(_message.Message):
        __slots__ = ["floorNumber"]
        FLOORNUMBER_FIELD_NUMBER: _ClassVar[int]
        floorNumber: int
        def __init__(self, floorNumber: _Optional[int] = ...) -> None: ...
    class Request(_message.Message):
        __slots__ = []
        def __init__(self) -> None: ...
    class Response(_message.Message):
        __slots__ = ["sensorName"]
        SENSORNAME_FIELD_NUMBER: _ClassVar[int]
        sensorName: str
        def __init__(self, sensorName: _Optional[str] = ...) -> None: ...
    def __init__(self) -> None: ...

class ImageMessages(_message.Message):
    __slots__ = []
    class ImageData(_message.Message):
        __slots__ = ["data", "format", "height", "width"]
        DATA_FIELD_NUMBER: _ClassVar[int]
        FORMAT_FIELD_NUMBER: _ClassVar[int]
        HEIGHT_FIELD_NUMBER: _ClassVar[int]
        WIDTH_FIELD_NUMBER: _ClassVar[int]
        data: bytes
        format: str
        height: int
        width: int
        def __init__(self, data: _Optional[bytes] = ..., format: _Optional[str] = ..., width: _Optional[int] = ..., height: _Optional[int] = ...) -> None: ...
    class ImageRequest(_message.Message):
        __slots__ = ["height", "imagePath", "width"]
        HEIGHT_FIELD_NUMBER: _ClassVar[int]
        IMAGEPATH_FIELD_NUMBER: _ClassVar[int]
        WIDTH_FIELD_NUMBER: _ClassVar[int]
        height: int
        imagePath: str
        width: int
        def __init__(self, imagePath: _Optional[str] = ..., width: _Optional[int] = ..., height: _Optional[int] = ...) -> None: ...
    class KongefamilieRequest(_message.Message):
        __slots__ = []
        def __init__(self) -> None: ...
    def __init__(self) -> None: ...

class NetatmoMessages(_message.Message):
    __slots__ = []
    class IndoorData(_message.Message):
        __slots__ = ["CO2", "humidity", "noise", "pressure", "temperature"]
        CO2: float
        CO2_FIELD_NUMBER: _ClassVar[int]
        HUMIDITY_FIELD_NUMBER: _ClassVar[int]
        NOISE_FIELD_NUMBER: _ClassVar[int]
        PRESSURE_FIELD_NUMBER: _ClassVar[int]
        TEMPERATURE_FIELD_NUMBER: _ClassVar[int]
        humidity: float
        noise: float
        pressure: float
        temperature: float
        def __init__(self, temperature: _Optional[float] = ..., CO2: _Optional[float] = ..., humidity: _Optional[float] = ..., noise: _Optional[float] = ..., pressure: _Optional[float] = ...) -> None: ...
    class NetatmoData(_message.Message):
        __slots__ = ["indoor", "outdoor"]
        INDOOR_FIELD_NUMBER: _ClassVar[int]
        OUTDOOR_FIELD_NUMBER: _ClassVar[int]
        indoor: NetatmoMessages.IndoorData
        outdoor: NetatmoMessages.OutdoorData
        def __init__(self, indoor: _Optional[_Union[NetatmoMessages.IndoorData, _Mapping]] = ..., outdoor: _Optional[_Union[NetatmoMessages.OutdoorData, _Mapping]] = ...) -> None: ...
    class NetatmoRequest(_message.Message):
        __slots__ = []
        def __init__(self) -> None: ...
    class OutdoorData(_message.Message):
        __slots__ = ["humidity", "temperature"]
        HUMIDITY_FIELD_NUMBER: _ClassVar[int]
        TEMPERATURE_FIELD_NUMBER: _ClassVar[int]
        humidity: float
        temperature: float
        def __init__(self, temperature: _Optional[float] = ..., humidity: _Optional[float] = ...) -> None: ...
    def __init__(self) -> None: ...

class SolarPanelMessages(_message.Message):
    __slots__ = []
    class PanelInfoRequest(_message.Message):
        __slots__ = ["azimuth", "datetime", "panelHeight", "panelName", "panelWidth", "tilt"]
        AZIMUTH_FIELD_NUMBER: _ClassVar[int]
        DATETIME_FIELD_NUMBER: _ClassVar[int]
        PANELHEIGHT_FIELD_NUMBER: _ClassVar[int]
        PANELNAME_FIELD_NUMBER: _ClassVar[int]
        PANELWIDTH_FIELD_NUMBER: _ClassVar[int]
        TILT_FIELD_NUMBER: _ClassVar[int]
        azimuth: float
        datetime: _timestamp_pb2.Timestamp
        panelHeight: float
        panelName: str
        panelWidth: float
        tilt: float
        def __init__(self, panelName: _Optional[str] = ..., panelWidth: _Optional[float] = ..., panelHeight: _Optional[float] = ..., tilt: _Optional[float] = ..., azimuth: _Optional[float] = ..., datetime: _Optional[_Union[_timestamp_pb2.Timestamp, _Mapping]] = ...) -> None: ...
    class PanelInfoResponse(_message.Message):
        __slots__ = ["currentIrradiance", "currentPower", "effeciency"]
        CURRENTIRRADIANCE_FIELD_NUMBER: _ClassVar[int]
        CURRENTPOWER_FIELD_NUMBER: _ClassVar[int]
        EFFECIENCY_FIELD_NUMBER: _ClassVar[int]
        currentIrradiance: float
        currentPower: float
        effeciency: float
        def __init__(self, currentPower: _Optional[float] = ..., currentIrradiance: _Optional[float] = ..., effeciency: _Optional[float] = ...) -> None: ...
    def __init__(self) -> None: ...

class TibberMessages(_message.Message):
    __slots__ = []
    class TimeResolution(int, metaclass=_enum_type_wrapper.EnumTypeWrapper):
        __slots__ = []
    class Request(_message.Message):
        __slots__ = ["timeResolution", "timeUnits"]
        TIMERESOLUTION_FIELD_NUMBER: _ClassVar[int]
        TIMEUNITS_FIELD_NUMBER: _ClassVar[int]
        timeResolution: TibberMessages.TimeResolution
        timeUnits: int
        def __init__(self, timeResolution: _Optional[_Union[TibberMessages.TimeResolution, str]] = ..., timeUnits: _Optional[int] = ...) -> None: ...
    class Response(_message.Message):
        __slots__ = ["image"]
        IMAGE_FIELD_NUMBER: _ClassVar[int]
        image: ImageMessages.ImageData
        def __init__(self, image: _Optional[_Union[ImageMessages.ImageData, _Mapping]] = ...) -> None: ...
    DAY: TibberMessages.TimeResolution
    HOUR: TibberMessages.TimeResolution
    MONTH: TibberMessages.TimeResolution
    def __init__(self) -> None: ...
