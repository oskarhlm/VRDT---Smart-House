protoc="C:\grpc2260\tools\windows_x64\protoc"
grpc_csharp_plugin="C:\grpc2260\tools\windows_x64\grpc_csharp_plugin.exe"
# protocc -I ../../protos/ --csharp_out=grpc/client --grpc_out=grpc/client --plugin=protoc-gen-grpc="C:\grpc2260\tools\windows_x64\grpc_csharp_plugin.exe" netatmo.proto
$protoc -I ../../protos/ --csharp_out=grpc/client --grpc_out=grpc/client --plugin=protoc-gen-grpc=$grpc_csharp_plugin netatmo.proto