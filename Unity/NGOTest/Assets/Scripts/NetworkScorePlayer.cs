using UnityEngine;
using Unity.Netcode;

public class NetworkScorePlayer : NetworkBehaviour
{
    public NetworkVariable<int> Score = new(0);

    public void RequestAddScore(int deltaScore)
    {
        if (!IsServer)
        {
            return;
        }

        int oldScore = Score.Value;
        Score.Value += deltaScore;
        Debug.Log($"<{OwnerClientId}> 클라이언트가 서버에 점수 변경을 요청하였습니다. [{oldScore} -> {Score.Value}]");

        UpdateOwnerScoreRPC(Score.Value);
    }

    [Rpc(SendTo.Owner)]
    private void UpdateOwnerScoreRPC(int score)
    {        
        NetworkLauncher.Instance.Score.SetText("{0}", score);
    }
}
