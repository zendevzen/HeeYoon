using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private static NetworkManager _instance;

    public static NetworkManager Instance => _instance;

    public PhotonView _photonView;

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
        
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"IsStart", true}});

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
                        // Debug.LogError($"모든 플레이어가 준비 완료!");
                        TaskManager.Instance.startPage.ShowButton(false, true);
                    }

                    // 선생일때
                    if (TaskManager.Instance.isTeacher)
                    {
                        if (PhotonNetwork.CurrentRoom.Players.Values.Any(i =>
                                i.CustomProperties.ContainsKey("IsStart")))
                        {
                            // 학생이 시작버튼을 누른 상태라면.
                            if (PhotonNetwork.CurrentRoom.Players.Values.Any(i => (bool) i.CustomProperties["IsStart"]))
                            {
                                TaskManager.Instance.CurrentTaskState = TaskManager.TaskState.Match;
                                StartSync();
                            }
                        }
                    }
                }
            }
                break;

            case SyncState.Sync:
            {
                // Expert 인 경우
                if (TaskManager.Instance.isTeacher && _timer > 1f)
                {
                    _timer = 0;
                    SendAnimationData();
                }

                // Worker 인 경우
                if (!TaskManager.Instance.isTeacher && _timer > 1f)
                {
                    // TODO : 큐에 쌓인거 하나씩 시간차 두고하기
                    
                    
                }
            }
                break;
        }

        _timer += Time.deltaTime;
    }

    

    // 정보 전달하기.. 어떤식으로 전달하지 json? list?
    private void SendAnimationData()
    {
        // 어떤 오브젝트 끼리 어떤 상호작용이 일어나고 있는지 내용을 보내줘야함.

        //TODO : 트리밍 해줘야함. 같은거 여러번 가거나 하는걸 막아야함.. 어케하지 1초마다니까 같은거 검색해서 하나뺴고 다지워 근데 중간에 다른거 껴있으면 앞에거지우나 뒤에꺼 지우나. 낀건 오류일 확률이 높겠지 1초니까

        if (TaskManager.Instance.animationDataList.Count > 0)
        {
            var jsonString = JsonConvert.SerializeObject(TaskManager.Instance.animationDataList);
            photonView.RPC("GetAnimationData", RpcTarget.Others, jsonString);
        }
    }

    [PunRPC]
    public void GetAnimationData(string jsonString)
    {
        Debug.LogError($"GetAnimationData 호출됨 {jsonString}");
        
        var dataList = JsonConvert.DeserializeObject<List<TaskManager.AnimationData>>(jsonString);

        if (dataList?.Count > 0)
        {
            foreach (var data in dataList)
            {
                TaskManager.Instance.animationDataList.Add(data);
            }
        }
    }

    public void SendMatchDone()
    {
        _photonView.RPC("AnnounceMatchDone", RpcTarget.Others);
    }
    
    [PunRPC]
    public void AnnounceMatchDone()
    {
        Debug.LogError("RPC로 AnnounceMatchDone 호출됨");
        
        TaskManager.Instance.CurrentTaskState = TaskManager.TaskState.Play;
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

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"IsReady", false}});
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"IsStart", false}});
        
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
