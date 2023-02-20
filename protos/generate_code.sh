proto_folder=.

# python_out_path=../python/grpc
python_out_path=../python
python_activate_path="C:\Users\oskar\.virtualenvs\vrdt-venv\Scripts\activate"

protoc="C:\grpc2260\tools\windows_x64\protoc"
grpc_csharp_plugin="C:\grpc2260\tools\windows_x64\grpc_csharp_plugin.exe"
csharp_out_path=../dotnet/Unity/Assets/Scripts/Grpc

source $python_activate_path
python -m grpc_tools.protoc -I $proto_folder --python_out=$python_out_path --pyi_out=$python_out_path --grpc_python_out=$python_out_path *.proto
echo "Generated Python code"
deactivate

$protoc -I $proto_folder --csharp_out=$csharp_out_path --grpc_out=$csharp_out_path --plugin=protoc-gen-grpc=$grpc_csharp_plugin *.proto
echo "Generated C# code"
