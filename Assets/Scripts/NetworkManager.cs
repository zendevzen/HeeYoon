using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private static NetworkManager _instance;

    public static NetworkManager Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    public enum SyncState
    {
        None,
        Wait,
        Sync,
    }

    private SyncState _currentSyncState;

    public SyncState CurrentSyncState
    {
        get => _currentSyncState;
        set
        {
            Debug.LogError($"State 변경 {_currentSyncState} = > {value}");
            _currentSyncState = value;
            OnChangeCurrentSyncState();
        }
    }

    private void OnChangeCurrentSyncState()
    {
        switch (CurrentSyncState)
        {
            case SyncState.None:
            {
                TaskManager.Instance.startPage.ShowButton(false, false);
            }
                break;
            
            case SyncState.Wait:
            {
                PrepareForSync();
            }
                break;

            case SyncState.Sync:
            {
                // Sync 시작
            }
                break;
        }
    }
    private void PrepareForSync()
    {
        // TODO : 카메라 나 마커 인식이 다 되었는지 체크해야함..

        TaskManager.Instance.startPage.ShowButton(true, false);
    }

    public void AnnounceReady()
    {
        // 상대 플레이어에게 준비 완료를 알림
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"IsReady", true}});
        
        TaskManager.Instance.startPage.ShowButton(false, false);
    }

    public void StartSync()
    {
        Debug.LogError($"Sync를 시작합니다.");

        CurrentSyncState = SyncState.Sync;
    }
    
    private float _timer = 0f;

    private void Update()
    {
        switch (CurrentSyncState)
        {
            case SyncState.Wait:
            {
                // 플레이어가 나 혼자일 경우 sync단계로 넘어가지 않는다.
                if (PhotonNetwork.CurrentRoom.Players.Count < 2)
                {
                    return;
                }

                // 모든 플레이어가 키를 갖고 있는지
                if (PhotonNetwork.CurrentRoom.Players.Values.All(i => i.CustomProperties.ContainsKey("IsReady")))
                {
                    foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
                    {
                        if ((bool) player.CustomProperties["IsReady"])
                        {
                            if (player.IsLocal)
                            {
                                if (TaskManager.Instance.isTeacher)
                                {
                                    TaskManager.Instance.startPage.SetReadyTeacher(true);
                                }
                                else
                                {
                                    TaskManager.Instance.startPage.SetReadyStudent(true);
                                }
                            }
                            else
                            {
                                if (TaskManager.Instance.isTeacher)
                                {
                                    TaskManager.Instance.startPage.SetReadyStudent(true);
                                }
                                else
                                {
                                    TaskManager.Instance.startPage.SetReadyTeacher(true);
                                }
                            }
                        }
                    }
                    
                    // 모두 준비가 됐으면 state를 sync로 변경
                    if (PhotonNetwork.CurrentRoom.Players.Values.All(i => (bool) i.CustomProperties["IsReady"]))
                    {
                        Debug.LogError($"모든 플레이어가 준비 완료!");
                        TaskManager.Instance.startPage.ShowButton(false, true);
                    }
                }
            }
                break;

            case SyncState.Sync:
            {
                // Expert 인 경우
                if (TaskManager.Instance.isTeacher && _timer > 0.1f)
                {
                    _timer = 0;
                    SendSyncData();
                }

                // Worker 인 경우
                else
                {
                }
            }
                break;
        }

        _timer += Time.deltaTime;
    }
    
    // 정보 전달하기.. 어떤식으로 전달하지 json? list?
    private void SendSyncData()
    {
        // 현재 뭘 보내줘야 하는지?
        
        
        
        
        /*switch (currentTaskLevel)
        {
            case TaskLevel.Second:
            {
                UpdatePosList();

                var jsonString = JsonConvert.SerializeObject(posListForExpert);
                
                photonView.RPC("SyncObjectPosition", RpcTarget.Others, jsonString);
            }
                break;

            case TaskLevel.Third:
            {
                UpdateRatioList();

                var jsonString = JsonConvert.SerializeObject(distanceRatioListForExpert);
                
                photonView.RPC("SyncObjectPosition", RpcTarget.Others, jsonString);
            }
                break;
        }*/
    }
    
    private void Start()
    {
        CurrentSyncState = SyncState.None;
        
        Connect();
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinOrCreateRoom("nooyix", new RoomOptions(), TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

        PhotonNetwork.JoinOrCreateRoom("nooyix", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.LogError("Room에 입장했습니다.");

        CurrentSyncState = SyncState.Wait;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}",
            cause);

        PhotonNetwork.LeaveRoom();
    }

    private void OnApplicationQuit()
    {
        PhotonNetwork.LeaveRoom();
    }
}
