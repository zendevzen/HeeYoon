using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using System.IO;
using System.Runtime.InteropServices;
//using ExitGames.Client.Photon.StructWrapping;
//using Microsoft.Azure.Kinect.Sensor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SocketManager : MonoBehaviour
{
    private TcpClient client;
    private string serverIP = "127.0.0.1";
    private int port = 8000;
    private byte[] receivedBuffer;
    private StreamReader reader;
    private bool socketReady = false;
    private NetworkStream stream;
    
    
    private List<DetectedData> detectedDataList = new List<DetectedData>();
    

    public GameObject augmentedObjectPrefab;
    
    public List<GameObject> augmentedObjectList = new List<GameObject>();
    
    
    private static SocketManager _instance;

    public static SocketManager Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }
    
    private float _bx = 0.3f;
    private float _by = -0.3f; //TaskManager.Instance.workPlacePos.y; // 손높이로하자
    private float _bz = 0.3f;

    public void SetWorkSpaceHeight(float height)
    {
        _by = height;
    }
    
    void AugmentingObject()
    {
        foreach (var t in augmentedObjectList)
        {
            t.SetActive(false);
        }

        for (var i = 0; i < detectedDataList.Count; i++)
        {
            if (i > augmentedObjectList.Count - 1)
            {
                augmentedObjectList.Add(Instantiate(augmentedObjectPrefab));
            }
            
            var _x = (detectedDataList[i].x_0 + detectedDataList[i].x_1)/2000;
            var _y = -(detectedDataList[i].y_0 + detectedDataList[i].y_1)/2000;
            
            augmentedObjectList[i].SetActive(true);
            augmentedObjectList[i].transform.position = new Vector3(-_x+_bx,_by,-_y+_bz);
            augmentedObjectList[i].GetComponent<AugmentedObject>().SetObjectData(detectedDataList[i].name);
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
        catch(Exception e)
        {
            Debug.Log("On client connect exception "+e);
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

                if (TaskManager.Instance.CurrentTaskState == TaskManager.TaskState.Play)
                {
                    AugmentingObject();
                }
            }
        }
        else
        {
            CheckReceive();
        }
    }
}
