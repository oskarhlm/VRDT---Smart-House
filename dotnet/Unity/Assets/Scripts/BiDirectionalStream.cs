using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrpcClients;
using System;
using System.Threading.Tasks;
using Grpc.Core.Utils;
using System.Threading;
using GrpcBase;
using Grpc.Core;

public class BiDirectionalStream : MonoBehaviour
{
    private Disruptive.DisruptiveClient _disruptiveClient = Clients.Instance.Disruptive;
    private AsyncServerStreamingCall<DisruptiveMessages.Types.Response> _call;
   
    private async void Start()
    {
        var request = new DisruptiveMessages.Types.Request();      
        _call = _disruptiveClient.GetTemperatureStream(request);

        while (await _call.ResponseStream.MoveNext(CancellationToken.None))
        {
            var response = _call.ResponseStream.Current;
            Debug.Log(response.SensorName);
        }       
    }

    private void OnDestroy()
    {
        _call.Dispose();
    }
}
