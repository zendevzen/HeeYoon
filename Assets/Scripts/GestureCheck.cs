using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // grab한 상태에서 1초동안 손동작 그림
// 원 -> mix
// 아무것도 없거나 움직임 적으면 -> hold
    
// Ungrab -> put
    
// 지금은 움직임 돌아오는거 측정하고,
// 이후 모션 많아지면 그때 인식으로 바꾸기
public class GestureCheck : MonoBehaviour
{
    private float timer;
    private float waitingTime;
    private bool isMixed;
    private Vector3 initPosition;
    private Vector3 newPosition;
    private float MaxDistance;

    public float threadInitDistance;
    public float threadMaxDistance;
    public GameObject cubeGrabbed;
    public TextMesh stateText;

    public SocketManager socketReceiveScript;

    private bool _isGrab = false;

    private static GestureCheck _instance;
    

    /*private string CheckGrabedOject()
    {
        // 손위치랑 물건 겹 치는거 나오게 해야함
        var HandPosition = cubeGrabbed.transform.position;
        var KitchenPosition= socketReceiveScript.KitchenObject;
        foreach (var t in KitchenPosition)
        {
            //Debug.LogError("손: " + HandPosition.x);
            //Debug.LogError($"컵 {KitchenPosition[2].transform.position.x}");
            var boundX = 0.05f;
            var boundY = 0.05f;
            var boundZ = 0.05f;
            if (HandPosition.x-boundX < t.transform.position.x && HandPosition.x+boundX > t.transform.position.x)
                if (HandPosition.y-boundY < t.transform.position.y && HandPosition.y+boundY > t.transform.position.y)
                    if (HandPosition.z-boundZ < t.transform.position.z && HandPosition.z+boundZ > t.transform.position.z)
                    {
                        return t.name;
                    }
        }

        return "";
    }*/
    public static GestureCheck Instance
    {
        get => _instance;
        set => _instance = value;
    }

    private void Awake()
    {
        if (!ReferenceEquals(_instance, null))
        {
            Instance = this;
        }
    }
    

    /*public void SetGrabState(bool state)
    {
        _isGrab = state;
        string st = "";

        Debug.LogError($"state 변경 {_isGrab}");

        if (_isGrab)
        {
            initPosition = cubeGrabbed.transform.position;
            
            timer = 0.0f;
            waitingTime = 1f;
            isMixed = false;
            MaxDistance = 0.0f;
            
            st = CheckGrabedOject();
            stateText.text = "grab " + st;
            
        }
        else
        {
            stateText.text = "put " + st;
        }
    }*/


    void Update()
    {
        if (!_isGrab)
            return;
  
        timer += Time.deltaTime;
       
        if (timer >= waitingTime)
        {
            if (isMixed)
            {
                stateText.text = "MIX";
            }

            timer = 0;
            isMixed = false;
            MaxDistance = 0f;
        }
        else
        {
            newPosition = cubeGrabbed.transform.position;
            var distance = Vector3.Distance(initPosition, newPosition);
            // 지금이 가장 멀리있나
            if (MaxDistance < distance)
            {
                MaxDistance = distance;
            }

            // 가장 멀리 있는게 아니라면 아니라면 처음 위치랑 비슷한가
            else
            {
                // 처음 위치로 돌아왔나
                if ((distance < threadInitDistance) &&(MaxDistance > threadMaxDistance))
                {

                    isMixed = true;
                }
            }
        }
    }
}