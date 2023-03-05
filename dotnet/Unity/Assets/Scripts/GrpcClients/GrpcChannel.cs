using Grpc.Core;

namespace Assets.Scripts.GrpcClients
{
    internal sealed class GrpcChannel
    {
        internal Channel Channel { get; private set; }
        private readonly string _server = "127.0.0.1:50051";
        private static GrpcChannel _instance = null;

        private GrpcChannel()
        {
            var option = new ChannelOption("grpc.max_receive_message_length", 5 * 1024 * 1024);
            ChannelOption[] options = { option };
            Channel = new Channel(_server, ChannelCredentials.Insecure, options);
        }

        internal static GrpcChannel Instance
        {
            get
            {
                if (_instance is null) { _instance = new GrpcChannel(); }
                return _instance;
            }
        }
    }
}
