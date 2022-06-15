using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SocketManager : MonoBehaviour
{
    private TcpClient client;
    private string serverIP = "127.0.0.1";
    private int port = 8001;
    private byte[] receivedBuffer;
    private StreamReader reader;
    private bool socketReady = false;
    private NetworkStream stream;


    private List<DetectedData> detectedDataList = new List<DetectedData>();


    public GameObject augmentedObjectPrefab;

    public List<AugmentedObject> augmentedObjectList = new List<AugmentedObject>();


    private static SocketManager _instance;

    public static SocketManager Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    public float _bx = 0.3f;
    public float _by = 0f; //TaskManager.Instance.workPlacePos.y; // 손높이로하자
    public float _bz = -0.3f;


    public class DetectedData
    {
        public string name;
    
        public float x_0;
        public float y_0;
    
        public float x_1;
        public float y_1;
    }
    
    void AugmentingObject()
    {
        foreach (var t in augmentedObjectList)
        {
            t.gameObject.SetActive(false);
        }

        for (var i = 0; i < detectedDataList.Count; i++)
        {
            var matchItem = augmentedObjectList.Find(item => item.objectData?.Name == detectedDataList[i].name);

            if (ReferenceEquals(matchItem, null))
            {
                matchItem = augmentedObjectList.Find(item => ReferenceEquals(item.objectData, null));
            }

            if (ReferenceEquals(matchItem, null))
            {
                Debug.LogError($"새로만듬 {detectedDataList[i].name}");
                var go = Instantiate(augmentedObjectPrefab);
                matchItem = go.GetComponent<AugmentedObject>();
                augmentedObjectList.Add(matchItem);
            }
            
            var _x = (detectedDataList[i].x_0 + detectedDataList[i].x_1) / 2000;
            var _y = -(detectedDataList[i].y_0 + detectedDataList[i].y_1) / 2000;
            
            matchItem.gameObject.SetActive(true);
            matchItem.transform.position = TaskManager.Instance.workPlacePos + new Vector3(-_x + _bx, _by, -_y + _bz);
            
            matchItem.SetObjectData(detectedDataList[i].name);
            
            matchItem.ShowObject(true);
        }
    }

    private void CheckReceive()
    {
        if (socketReady)
        {
            return;
        }

        try
        {
            client = new TcpClient(serverIP, port);

            if (client.Connected)
            {
                stream = client.GetStream();
                Debug.Log("Connect Success");
                socketReady = true;
            }
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }

    private void CloseSocket()
    {
        if (!socketReady) return;
        reader.Close();
        client.Close();
        socketReady = false;
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    void Start()
    {
        CheckReceive();
    }

    void Update()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                detectedDataList.Clear();

                receivedBuffer = new byte[1440];
                stream.Read(receivedBuffer, 0, receivedBuffer.Length);
                var msg = Encoding.UTF8.GetString(receivedBuffer, 0, receivedBuffer.Length);

                msg = msg.Replace("\u0000", "");
                //Debug.Log(msg);
                var jsonStr = JsonConvert.DeserializeObject<JObject>(msg);

                foreach (var item in jsonStr["detectData"])
                {
                    var itemData = new DetectedData()
                    {
                        name = item["class"].ToString(),
                        x_0 = float.Parse(item["bbox"][0].ToString()),
                        y_0 = float.Parse(item["bbox"][1].ToString()),
                        x_1 = float.Parse(item["bbox"][2].ToString()),
                        y_1 = float.Parse(item["bbox"][3].ToString()),
                    };
                    //Debug.Log($"name {itemData.name} x0 {itemData.x_0} y0 {itemData.y_0} x1 {itemData.x_1} y1 {itemData.y_1}");

                    detectedDataList.Add(itemData);
                }

                AugmentingObject();
                /*if (TaskManager.Instance.CurrentTaskState == TaskManager.TaskState.Play)
                {
                    AugmentingObject();
                }*/
            }
        }
        else
        {
            CheckReceive();
        }
    }
}