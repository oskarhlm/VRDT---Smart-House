# Unity-gRPC: https://shadabambat1.medium.com/basic-client-server-communication-using-unity-grpc-f4a3c2cf819c
# Binaries: https://packages.grpc.io/archive/2019/11/6950e15882f28e43685e948a7e5227bfcef398cd-6d642d6c-a6fc-4897-a612-62b0a3c9026b/index.xml
# Grpc.Tools (correct version): https://www.nuget.org/packages/Grpc.Tools/2.26.0

. ../config.ini # Add python_activate_path, protoc and grpc_csharp_plugin here
proto_folder=.
python_out_path=../python
csharp_out_path=../dotnet/Unity/Assets/Scripts/GrpcGens


source $python_activate_path
python -m grpc_tools.protoc -I $proto_folder --python_out=$python_out_path --pyi_out=$python_out_path --grpc_python_out=$python_out_path base.proto
# $protoc --proto_path grpc_gens=$proto_folder --python_out $python_out_path $proto_folder/*.proto
echo "Generated Python code"

$protoc -I $proto_folder --proto_path=$proto_folder --csharp_out=$csharp_out_path --grpc_out=$csharp_out_path --plugin=protoc-gen-grpc=$grpc_csharp_plugin *.proto
echo "Generated C# code"
