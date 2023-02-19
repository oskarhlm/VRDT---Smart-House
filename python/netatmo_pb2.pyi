from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Mapping as _Mapping, Optional as _Optional, Union as _Union

DESCRIPTOR: _descriptor.FileDescriptor

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
    indoor: IndoorData
    outdoor: OutdoorData
    def __init__(self, indoor: _Optional[_Union[IndoorData, _Mapping]] = ..., outdoor: _Optional[_Union[OutdoorData, _Mapping]] = ...) -> None: ...

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
