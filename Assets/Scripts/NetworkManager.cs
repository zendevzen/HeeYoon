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
                if (TaskManager.Instance.isTeacher && _timer > 6f)
                {
                    _timer = 0;
                    SendAnimationData();
                }

                // Worker 인 경우
                if (!TaskManager.Instance.isTeacher && _timer > 5f)
                {
                    if (!AnimationManager.Instance.isPlaying)
                    {
                        if (TaskManager.Instance.animationDataList.Count > 0)
                        {
                            var animationData = TaskManager.Instance.animationDataList[0];
                            
                            var mainObject = SocketManager.Instance.augmentedObjectList.Find(i =>
                                i.objectData.Name == animationData.MainName);

                            if (ReferenceEquals(mainObject, null))
                            {
                                if (!TaskManager.Instance.isTeacher)
                                {
                                    var idx = TaskManager.Instance.teacherObjectDataList.FindIndex(i =>
                                        i.Name == animationData.MainName);

                                    mainObject = SocketManager.Instance.augmentedObjectList.Find(i =>
                                        i.objectData.Name == TaskManager.Instance.studentObjectDataList[idx].Name);
                                }
                            }
                            
                            var subObject = SocketManager.Instance.augmentedObjectList.Find(i =>
                                i.objectData.Name == animationData.SubName);
                            
                            if (ReferenceEquals(subObject, null))
                            {
                                if (!TaskManager.Instance.isTeacher)
                                {
                                    var idx = TaskManager.Instance.teacherObjectDataList.FindIndex(i =>
                                        i.Name == animationData.SubName);

                                    subObject = SocketManager.Instance.augmentedObjectList.Find(i =>
                                        i.objectData.Name == TaskManager.Instance.studentObjectDataList[idx].Name);
                                }
                            }
                            

                            /*Debug.LogError($"SocketManager.Instance.augmentedObjectList {SocketManager.Instance.augmentedObjectList.Count}");
                            Debug.LogError($"mainObject {mainObject}");
                            Debug.LogError($"subObject {subObject}");*/
                            
                            if (!ReferenceEquals(mainObject, null) && !ReferenceEquals(subObject, null))
                            {
                                //Debug.LogError($"animationData {mainObject.objectData.Name} {subObject.objectData.Name} {animationData.Category}");
                                
                                // 이름으로 오브젝트 갖고와야함
                                TaskManager.Instance.animationDataList.RemoveAt(0);
                                
                                AnimationManager.Instance.PlayAnimation(mainObject, subObject, animationData.Category);
                            }
                        }
                    }
                    
                    _timer = 0f;
                }
            }
                break;
        }

        _timer += Time.deltaTime;
    }

    

    // 정보 전달하기.. 어떤식으로 전달하지 json? list?
    private void SendAnimationData()
    {
        if (TaskManager.Instance.animationDataList.Count > 0)
        {
            var jsonString = JsonConvert.SerializeObject(TaskManager.Instance.animationDataList);
            photonView.RPC("GetAnimationData", RpcTarget.Others, jsonString);
            
            TaskManager.Instance.animationDataList.Clear();
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
                TaskManager.Instance.AddAnimationData(data);
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
        
        //TODO : 없애기
        
        //CurrentSyncState = SyncState.Sync;
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
