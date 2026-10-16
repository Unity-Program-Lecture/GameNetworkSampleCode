using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkLauncher : MonoBehaviour
{
    public static NetworkLauncher Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Button addScore;

    public TextMeshProUGUI Score => score;
    public Button AddScore => addScore;

    private void Awake()
    {
        score.SetText("{0}", 0);

        Instance = this;
    }

    public void StartHost()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            return;
        }

        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            return;
        }

        NetworkManager.Singleton.StartClient();
    }
}
