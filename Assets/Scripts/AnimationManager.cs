using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    public GameObject augmentedObjectPrefab;
    public AugmentedObject mainObject;
    public AugmentedObject subObject;

    public bool isPlaying;

    private void Awake()
    {
        Instance = this;
        
        var mainGo = Instantiate(augmentedObjectPrefab, transform);
        mainObject = mainGo.GetComponent<AugmentedObject>();
        mainObject.ShowObject(false);
        
        var subGo = Instantiate(augmentedObjectPrefab, transform);
        subObject = subGo.GetComponent<AugmentedObject>();
        subObject.ShowObject(false);
    }

    /*public enum AnimationType
    {
        Put,
        Mix,
        Cut,
        Pour
    }*/

    public void PlayAnimation(AugmentedObject main, AugmentedObject target, TaskManager.AnimationCategory animationType)
    {
        Debug.LogError($"PlayAnimation {main.objectData.Name} {target.objectData.Name} {animationType}");
        
        switch (animationType)
        {
            case TaskManager.AnimationCategory.Put:
            {
                StartCoroutine(Co_Put(main, target));
            }
                break;

            case TaskManager.AnimationCategory.Mix:
            {
                StartCoroutine(Co_Mix(main, target));
            }
                break;
            
            case TaskManager.AnimationCategory.Cut:
            {
                StartCoroutine(Co_Cut(main, target));
            }
                break;
            
            case TaskManager.AnimationCategory.Pour:
            {
                StartCoroutine(Co_Pour(main, target));
            }
                break;
        }
    }
    
    private IEnumerator Co_Put(AugmentedObject main, AugmentedObject target)
    {
        //Debug.LogError("Co_Put 시작");
        isPlaying = true;

        mainObject.objectData = main.objectData;
        subObject.objectData = target.objectData;
        
        mainObject.ShowObject(true);
        subObject.ShowObject(true);

        var mainTransform = mainObject.transform;

        mainTransform.position = main.transform.position;
        
        var targetTransform = subObject.transform;
        
        targetTransform.position = target.transform.position;
        
        var targetPos = targetTransform.position;
        
        yield return new WaitForSecondsRealtime(1.5f);
        
        // 이동
        var firstPos = mainTransform.position;
        var secondPos = (firstPos + targetPos) / 2f + new Vector3(0f, 0.2f, 0f);
        var thirdPos = targetPos + new Vector3(0f, 0.1f, 0f);
        
        mainTransform.DOPath(new[] {firstPos, secondPos, thirdPos}, 3f,PathType.CatmullRom).SetEase(Ease.InQuad);
            
        yield return new WaitForSecondsRealtime(4f);

        // 행동
        
        mainTransform.DOPath(new[] {thirdPos, targetTransform.position}, 2f,PathType.Linear).SetEase(Ease.InQuad);
        
        yield return new WaitForSecondsRealtime(5f);
        
        mainObject.ShowObject(false);
        subObject.ShowObject(false);
        
        isPlaying = false;
    }
    
    
    private IEnumerator Co_Pour(AugmentedObject main, AugmentedObject target)
    {
        isPlaying = true;
        
        mainObject.objectData = main.objectData;
        subObject.objectData = target.objectData;
        
        mainObject.ShowObject(true);
        subObject.ShowObject(true);

        var mainTransform = mainObject.transform;

        mainTransform.position = main.transform.position;
        
        var targetTransform = subObject.transform;
        
        targetTransform.position = target.transform.position;
        
        var targetPos = targetTransform.position;
        
        mainTransform.LookAt(targetTransform);
        
        
        yield return new WaitForSecondsRealtime(1.5f);
        
        // 이동
        var firstPos = mainTransform.position;
        var secondPos = (firstPos + targetPos) / 2f + new Vector3(0f, 0.2f, 0f);
        var thirdPos = targetPos + new Vector3(0f, 0.2f, 0f);
        
        mainTransform.DOPath(new[] {firstPos, secondPos, thirdPos}, 3f,PathType.CatmullRom).SetEase(Ease.InQuad);
            
        yield return new WaitForSecondsRealtime(4f);

        // 행동
        
        mainTransform.DORotate(mainTransform.rotation.eulerAngles + new Vector3(180f, 0f, 0f), 2f, RotateMode.FastBeyond360);
            
        yield return new WaitForSecondsRealtime(4f);

        mainObject.ShowObject(false);
        subObject.ShowObject(false);
        
        isPlaying = false;
    }
    
    private IEnumerator Co_Mix(AugmentedObject main, AugmentedObject target)
    {
        isPlaying = true;
        
        mainObject.objectData = main.objectData;
        subObject.objectData = target.objectData;
        
        mainObject.ShowObject(true);
        subObject.ShowObject(true);


        var mainTransform = mainObject.transform;

        mainTransform.position = main.transform.position;
        
        var targetTransform = subObject.transform;
        
        targetTransform.position = target.transform.position;
        
        var targetPos = targetTransform.position;
        
        mainTransform.LookAt(targetTransform);
        
        
        yield return new WaitForSecondsRealtime(1.5f);

        // 이동
        var firstPos = mainTransform.position;
        var secondPos = (firstPos + targetPos) / 2f + new Vector3(0f, 0.2f, 0f);
        var thirdPos = targetPos + new Vector3(0f, 0.14f, 0f);
        
        mainTransform.DOPath(new[] {firstPos, secondPos, thirdPos}, 3f,PathType.CatmullRom).SetEase(Ease.InQuad);
            
        yield return new WaitForSecondsRealtime(4f);

        // 행동
        
        mainTransform.DORotate(mainTransform.rotation.eulerAngles + new Vector3(0f, 1440f, 0f), 4f, RotateMode.FastBeyond360);
            
        yield return new WaitForSecondsRealtime(4.5f);

        mainObject.ShowObject(false);
        subObject.ShowObject(false);
        
        isPlaying = false;
    }
    
    private IEnumerator Co_Cut(AugmentedObject main, AugmentedObject target)
    {
        isPlaying = true;
        
        mainObject.objectData = main.objectData;
        subObject.objectData = target.objectData;
        
        mainObject.ShowObject(true);
        subObject.ShowObject(true);
        

        var mainTransform = mainObject.transform;

        mainTransform.position = main.transform.position;
        
        var targetTransform = subObject.transform;
        
        targetTransform.position = target.transform.position;
        
        var targetPos = targetTransform.position;
        
        mainTransform.LookAt(targetTransform);
        targetTransform.LookAt(mainTransform);
        
        yield return new WaitForSecondsRealtime(1.5f);

        // 이동
        var firstPos = mainTransform.position;
        var secondPos = (firstPos + targetPos) / 2f + new Vector3(0f, 0.2f, 0f);
        var thirdPos = targetPos + new Vector3(0f, 0.14f, 0f);
        
        mainTransform.DOPath(new[] {firstPos, secondPos, thirdPos}, 3f,PathType.CatmullRom).SetEase(Ease.InQuad);
            
        yield return new WaitForSecondsRealtime(4f);

        // 행동
        mainTransform.DOPath(new[] {thirdPos , targetPos, thirdPos, targetPos, thirdPos, targetPos, thirdPos, targetPos, thirdPos, thirdPos}, 3f,PathType.Linear).SetEase(Ease.Linear);
        mainTransform.Rotate(0f, -30f, 0f);
        mainTransform.DORotate(mainTransform.rotation.eulerAngles + new Vector3(0f, 60f, 0f), 3.5f, RotateMode.FastBeyond360);
            
        yield return new WaitForSecondsRealtime(4.5f);

        mainObject.ShowObject(false);
        subObject.ShowObject(false);
        
        isPlaying = false;
    }
    
}