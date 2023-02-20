using Grpc.Core;

namespace Unity_Utils
{
    public class NetatmoClient
    {
        private Channel channel;
        //public Netatmo.Netatmo.NetatmoClient client { get; private set; }

        public NetatmoClient()
        {
            channel = new Channel("https://localhost:50051", ChannelCredentials.Insecure);
            //client = new Netatmo.Netatmo.NetatmoClient(channel);
        }       
    }
}