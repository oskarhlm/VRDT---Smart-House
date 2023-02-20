proto_folder=../protos
proto_path=$proto_folder/netatmo.proto
output_path=.

<<<<<<< HEAD
python -m grpc_tools.protoc -I$proto_folder --python_out=$output_path --pyi_out=$output_path --grpc_python_out=$output_path $proto_path
=======
python -m grpc_tools.protoc -I../protos --python_out=$output_path --pyi_out=$output_path --grpc_python_out=$output_path $proto_path
>>>>>>> bea226c2c474dd810521910e5b9543e83a6db650
