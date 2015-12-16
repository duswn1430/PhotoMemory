using UnityEngine;
using System.Collections;
using System;

public class GameService : MonoBehaviour
{
    private static GameService instance = null;
    public static GameService _Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (new GameObject("GameService")).AddComponent<GameService>();
            }
            return instance;
        }
    }

    public event Action HandlePlayerConnected;
    public event Action HandlePlayerDisconnected;

    //public event Action HandleScoreSubmitted;

    string _sBoard = "score";

    // Use this for initialization
    void Start()
    {
        UM_GameServiceManager.OnPlayerConnected += OnPlayerConnected;
        UM_GameServiceManager.OnPlayerDisconnected += OnPlayerDisconnected;
    }

    public void Connect()
    {
        UM_GameServiceManager.instance.Connect();
    }

    public void Disconnect()
    {
        UM_GameServiceManager.instance.Disconnect();
    }

    private void OnPlayerConnected()
    {
        Debug.Log("Player Connected");

        if (HandlePlayerConnected != null)
        {
            HandlePlayerConnected();
            HandlePlayerConnected = null;
        }
    }

    private void OnPlayerDisconnected()
    {
        Debug.Log("Player Disconnected");

        if (HandlePlayerDisconnected != null)
        {
            HandlePlayerDisconnected();
            HandlePlayerDisconnected = null;
        }
    }

    public void ShowLeaderBoard()
    {
        UM_GameServiceManager.instance.ShowLeaderBoardUI(_sBoard);
    }

    public void ScoreSubmit(int score)
    {
        UM_GameServiceManager.instance.SubmitScore(_sBoard, score);
    }
}
