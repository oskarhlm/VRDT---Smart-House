using Assets.Scripts.GrpcClients;
using Grpc.Core;
using GrpcBase;

namespace Assets.Scripts
{
    public sealed class Clients
    {
        private Channel _channel = GrpcChannel.Instance.Channel;
        public readonly Netatmo.NetatmoClient Netatmo;
        public readonly Image.ImageClient Image;
        private static Clients _instance = null;

        private Clients()
        {
            Netatmo = new Netatmo.NetatmoClient(_channel);
            Image = new Image.ImageClient(_channel);
        }

        internal static Clients Instance
        {
            get
            {
                if (_instance is null) { _instance = new Clients(); }
                return _instance;
            }
        }
    }
}
