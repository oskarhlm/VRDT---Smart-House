using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrpcClients;
using System;
using System.Threading.Tasks;
using Grpc.Core.Utils;
using System.Threading;
using GrpcBase;

public class BiDirectionalStream : MonoBehaviour
{
    private GrpcBase.Disruptive.DisruptiveClient _disruptiveClient = Clients.Instance.Disruptive;

    // Start is called before the first frame update
    async Task WatchForChangesAsync()
    {
        using var call = _disruptiveClient.GetTemperatureStream();

        await call.RequestStream.WriteAsync(new DisruptiveMessages.Types.Request());

        Debug.Log("Starting background task to receive messages");
        var readStream = Task.Run(async () =>
        {
            Debug.Log("hei");
            while (await call.ResponseStream.MoveNext(CancellationToken.None))
            {
                var res = call.ResponseStream.Current;
                Debug.Log(res.SensorName);
            }
        });

    }

    //private IEnumerator Start()
    //{
    //    yield return StartCoroutine(WatchForChangesAsync());
    //}
}
