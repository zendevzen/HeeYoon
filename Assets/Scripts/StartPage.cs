using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StartPage : MonoBehaviour
{
    public GameObject workspaceSettingPanel;
    public GameObject checkConnectionPanel;

    public enum StartPageState
    {
        WorkSpaceSetting,
        CheckConnection,
    }

    private StartPageState _currentStartPageState;

    public StartPageState CurrentStartPageState
    {
        get => _currentStartPageState;
        set
        {
            _currentStartPageState = value;
            OnCurrentStartPageStateChanged();
        }
    }

    private void OnCurrentStartPageStateChanged()
    {
        Debug.LogError($"CurrentStartPageState {CurrentStartPageState}");

        switch (CurrentStartPageState)
        {
            case StartPageState.WorkSpaceSetting:
            {
                workspaceSettingPanel.SetActive(true);
                checkConnectionPanel.SetActive(false);

                TaskManager.Instance.FixCanvasToHead(true);
            }
                break;
            
            case StartPageState.CheckConnection:
            {
                workspaceSettingPanel.SetActive(false);
                checkConnectionPanel.SetActive(true);
                
                TaskManager.Instance.FixCanvasToHead(false);
            }
                break;
        }
    }
    
    public void Init()
    {
        SetReadyTeacher(false);
        SetReadyStudent(false);

        ShowButton(false, false);

        if (TaskManager.Instance.isTeacher)
        {
            CurrentStartPageState = StartPageState.CheckConnection;

            return;
        }

        leftProgressImage.fillAmount = 0f;
        leftFistText.gameObject.SetActive(true);
        rightProgressImage.fillAmount = 0f;
        rightFistText.gameObject.SetActive(true);

        doneText.gameObject.SetActive(false);
        
        _isLeftFistDone = false;
        _isRightFistDone = false;
        _isDone = false;

        CurrentStartPageState = StartPageState.WorkSpaceSetting;
    }
    
    
    // workspace setting 관련

    public GameObject leftHandPosGo;
    public GameObject rightHandPosGo;
    
    public Image leftProgressImage;
    public Text leftFistText;
    
    public Image rightProgressImage;
    public Text rightFistText;

    public Text doneText;

    private bool _isLeftFistDone;
    private bool _isRightFistDone;

    private bool _isDone;

    // public GameObject mapGo;
    
    public void OnLeftFistGrip()
    {
        if (TaskManager.Instance.CurrentTaskState != TaskManager.TaskState.Ready)
            return;
        
        if (CurrentStartPageState != StartPageState.WorkSpaceSetting)
        {
            return;
        }
        
        leftProgressImage.fillAmount = 0f;
        
        leftFistText.gameObject.SetActive(false);
        
        StartCoroutine(nameof(Co_LeftFist));
    }

    public void OnLeftFistRelease()
    {
        if (TaskManager.Instance.CurrentTaskState != TaskManager.TaskState.Ready)
            return;
        
        if (CurrentStartPageState != StartPageState.WorkSpaceSetting)
        {
            return;
        }
        
        if (_isDone)
            return;
        
        StopCoroutine(nameof(Co_LeftFist));
        leftProgressImage.fillAmount = 0f;
        leftFistText.gameObject.SetActive(true);
        
        _isLeftFistDone = false;
    }
    
    public void OnRightFistGrip()
    {
        if (TaskManager.Instance.CurrentTaskState != TaskManager.TaskState.Ready)
            return;
        
        if (CurrentStartPageState != StartPageState.WorkSpaceSetting)
        {
            return;
        }
        
        rightProgressImage.fillAmount = 0f;
        
        rightFistText.gameObject.SetActive(false);
        
        StartCoroutine(nameof(Co_RightFist));
    }

    public void OnRightFistRelease()
    {
        if (TaskManager.Instance.CurrentTaskState != TaskManager.TaskState.Ready)
            return;

        if (CurrentStartPageState != StartPageState.WorkSpaceSetting)
        {
            return;
        }
        
        if (_isDone)
            return;
        
        StopCoroutine(nameof(Co_RightFist));
        rightProgressImage.fillAmount = 0f;
        rightFistText.gameObject.SetActive(true);
        
        _isRightFistDone = false;
    }


    private IEnumerator Co_FistDone()
    {
        _isDone = true;

        leftProgressImage.fillAmount = 0f;
        rightProgressImage.fillAmount = 0f;
        
        doneText.gameObject.SetActive(true);

        TaskManager.Instance.workPlacePos = (leftHandPosGo.transform.position + rightHandPosGo.transform.position) / 2f;
        TaskManager.Instance.workPlacePos.y -= 0.05f;
        // 작업공간의 높이를 설정해준다.
        //SocketManager.Instance.SetWorkSpaceHeight(TaskManager.Instance.workPlacePos.y);
        
        Debug.LogError($"workPlacePos {TaskManager.Instance.workPlacePos}");

        //mapGo.transform.position = TaskManager.Instance.workPlacePos;
        //mapGo.transform.LookAt(TaskManager.Instance.headPosTransform);
        //var angle = mapGo.transform.rotation.eulerAngles;
        //var yVal = angle.y;
        //mapGo.transform.rotation = Quaternion.Euler(0f, yVal, 0f);
        
        yield return new WaitForSecondsRealtime(3f);

        CurrentStartPageState = StartPageState.CheckConnection;
    }

    private IEnumerator Co_LeftFist()
    {
        var timer = 0f;
        
        while (timer < 3f)
        {
            yield return null;
            
            timer += Time.deltaTime;
            
            leftProgressImage.fillAmount = timer / 3f;
        }

        leftProgressImage.fillAmount = 1f;
        _isLeftFistDone = true;

        if (_isRightFistDone)
        {
            StartCoroutine(nameof(Co_FistDone));
        }
    }
    
    private IEnumerator Co_RightFist()
    {
        var timer = 0f;
        
        while (timer < 3f)
        {
            yield return null;
            
            timer += Time.deltaTime;
            
            rightProgressImage.fillAmount = timer / 3f;
        }

        rightProgressImage.fillAmount = 1f;
        _isRightFistDone = true;
        
        if (_isLeftFistDone)
        {
            StartCoroutine(nameof(Co_FistDone));
        }
    }
    
    
    // check connection 관련
    
    public GameObject readyButton;
    public GameObject readyButtonBg;

    public GameObject startButton;
    public GameObject startButtonBg;

    public Text teacherReadyText;
    public Text studentReadyText;
    
    public void SetReadyTeacher(bool isReady)
    {
        teacherReadyText.text = isReady ? "Ready.." : "Wait..";
        teacherReadyText.color = isReady ? Color.green : Color.black;
    }
    
    public void SetReadyStudent(bool isReady)
    {
        studentReadyText.text = isReady ? "Ready.." : "Wait..";
        studentReadyText.color = isReady ? Color.green : Color.black;
    }

    public void ShowButton(bool ready, bool start)
    {
        readyButton.SetActive(ready);
        readyButtonBg.SetActive(!ready);
        
        startButton.SetActive(start);
        startButtonBg.SetActive(!start);

        if (TaskManager.Instance.isTeacher)
        {
            startButton.SetActive(false);
            startButtonBg.SetActive(false);
        }
    }
    
    public void OnReadyButtonClicked()
    {
        NetworkManager.Instance.AnnounceReady();

        if (TaskManager.Instance.isTeacher)
        {
            SetReadyTeacher(true);
        }
        else
        {
            SetReadyStudent(true);
        }
    }
    
    public void OnStartButtonClicked()
    {
        Debug.LogError("스탓버튼");
        TaskManager.Instance.CurrentTaskState = TaskManager.TaskState.Match;
        NetworkManager.Instance.StartSync();
    }
}
