using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectMatchPage : MonoBehaviour
{
    public List<TextMeshPro> teacherButtonTextList = new List<TextMeshPro>();
    public List<TextMeshPro> studentButtonTextList = new List<TextMeshPro>();

    public GameObject fixModeTextGo;

    public GameObject forTeacherPanel;
    public GameObject forStudentPanel;

    public GameObject fixButtonGo;
    public GameObject doneButtonGo;

    private bool _isFixMode = false;

    public bool IsFixMode
    {
        get => _isFixMode;
        set
        {
            _isFixMode = value;
            OnIsFixModeChanged();
        }
    }

    public void Init()
    {
        if (TaskManager.Instance.isTeacher)
        {
            forTeacherPanel.SetActive(true);
            forStudentPanel.SetActive(false);
            
            fixButtonGo.SetActive(false);
            doneButtonGo.SetActive(false);
        }
        else
        {
            forTeacherPanel.SetActive(false);
            forStudentPanel.SetActive(true);
            
            fixButtonGo.SetActive(true);
            doneButtonGo.SetActive(true);
            
            IsFixMode = false;
        }
    }

    public void RefreshList()
    {
        for (var i = 0; i < TaskManager.Instance.teacherObjectList.Count; i++)
        {
            teacherButtonTextList[i].text = TaskManager.Instance.teacherObjectList[i];
            teacherButtonTextList[i].color = Color.black;

            studentButtonTextList[i].text = TaskManager.Instance.studentObjectList[i];
            studentButtonTextList[i].color = Color.black;
        }
        
        _currentSelectedIndex = -1;
    }

    private void OnIsFixModeChanged()
    {
        if (IsFixMode)
        {
            fixModeTextGo.SetActive(true);
            RefreshList();
        }
        else
        {
            fixModeTextGo.SetActive(false);
            RefreshList();
        }
    }

    public void FixModeButtonClicked()
    {
        IsFixMode = !IsFixMode;
    }

    private int _currentSelectedIndex = -1;

    public void StudentButtonClicked(int index)
    {
        if (_currentSelectedIndex < 0)
        {
            _currentSelectedIndex = index;
            studentButtonTextList[index].color = Color.green;
        }

        // 선택된 버튼이 있는경우
        else
        {
            (TaskManager.Instance.studentObjectList[_currentSelectedIndex],
                TaskManager.Instance.studentObjectList[index]) = (TaskManager.Instance.studentObjectList[index],
                TaskManager.Instance.studentObjectList[_currentSelectedIndex]);

            RefreshList();
        }
    }

    public void DoneButtonClicked()
    {
        TaskManager.Instance.CurrentTaskState = TaskManager.TaskState.Play;

        NetworkManager.Instance.SendMatchDone();
    }
}