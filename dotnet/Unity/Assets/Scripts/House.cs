using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using Grpc.Core;
using GrpcBase;

public class House : MonoBehaviour
{
    private readonly Netatmo.NetatmoClient _client;
    private readonly Channel _channel;
    private readonly string _server = "127.0.0.1:50051";

    public House()
    {
        _channel = new Channel(_server, ChannelCredentials.Insecure);
        _client = new Netatmo.NetatmoClient(_channel);
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            var child = gameObject.transform.GetChild(i).gameObject;
            //child.AddComponent<Rigidbody>();
            child.AddComponent<MeshCollider>();
            //child.layer = LayerMask.NameToLayer("House");
        }

        var data = _client.GetData(new NetatmoMessages.Types.NetatmoRequest());
        Debug.Log(data);
    }
}
