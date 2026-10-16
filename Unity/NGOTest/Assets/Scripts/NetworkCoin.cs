using System;
using System.Threading;
using UnityEngine;
using Unity.Netcode;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

public class NetworkCoin : NetworkBehaviour
{
    private CancellationTokenSource _ctsTriggerEnter;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _ctsTriggerEnter = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

            TriggerEnterAsync(_ctsTriggerEnter.Token).Forget();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (_ctsTriggerEnter is not null)
        {
            _ctsTriggerEnter.Cancel();
            _ctsTriggerEnter.Dispose();
            _ctsTriggerEnter = null;
        }
    }

    private async UniTask TriggerEnterAsync(CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            while (!cancellationToken.IsCancellationRequested)
            {
                Collider other = await this.GetAsyncTriggerEnterTrigger().OnTriggerEnterAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                if (!other.TryGetComponent(out NetworkScorePlayer networkScorePlayer))
                {
                    continue;
                }

                networkScorePlayer.RequestAddScore(1);

                break;
            }

        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            Debug.LogException(e, this);
        }
        finally
        {
            NetworkObject.Despawn();
        }
    }
}
