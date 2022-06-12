using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    // TODO : 인스턴스 만들기 싱글턴
    
    public Transform mainObject;
    public Transform targetObject;

    public bool isPlaying;
    
    public enum AnimationType
    {
        Put,
        Mix,
        Cut,
        Pour
    }

    public void PlayAnimation(Transform main, Transform target, AnimationType animationType)
    {
        switch (animationType)
        {
            case AnimationType.Put:
            {
                StartCoroutine(Co_Put(main, target));
            }
                break;

            case AnimationType.Mix:
            {
                StartCoroutine(Co_Mix(main, target));
            }
                break;
            
            case AnimationType.Cut:
            {
                StartCoroutine(Co_Cut(main, target));
            }
                break;
            
            case AnimationType.Pour:
            {
                StartCoroutine(Co_Pour(main, target));
            }
                break;
        }
    }
    
    private IEnumerator Co_Put(Transform main, Transform target)
    {
        isPlaying = true;
        
        var targetPos = target.position;
        
        // 이동
        var firstPos = main.position;
        var secondPos = (firstPos + targetPos) / 2f + new Vector3(0f, 0.2f, 0f);
        var thirdPos = targetPos + new Vector3(0f, 0.1f, 0f);
        
        main.DOPath(new[] {firstPos, secondPos, thirdPos}, 3f,PathType.CatmullRom).SetEase(Ease.InQuad);
            
        yield return new WaitForSecondsRealtime(4f);

        // 행동
        
        main.DOPath(new[] {thirdPos, target.position}, 2f,PathType.Linear).SetEase(Ease.InQuad);
        
        yield return new WaitForSecondsRealtime(5f);
        
        // 오브젝트 꺼주기
        
        isPlaying = false;
        
        // TODO : 오브젝트 어케 꺼줄지랑 플레이 중인지 판별하기
    }
    
    
    private IEnumerator Co_Pour(Transform main, Transform target)
    {
        var targetPos = target.position;
        
        main.LookAt(target);
        
        // 이동
        var firstPos = main.position;
        var secondPos = (firstPos + targetPos) / 2f + new Vector3(0f, 0.2f, 0f);
        var thirdPos = targetPos + new Vector3(0f, 0.2f, 0f);
        
        main.DOPath(new[] {firstPos, secondPos, thirdPos}, 3f,PathType.CatmullRom).SetEase(Ease.InQuad);
            
        yield return new WaitForSecondsRealtime(4f);

        // 행동
        
        main.DORotate(main.rotation.eulerAngles + new Vector3(180f, 0f, 0f), 2f, RotateMode.FastBeyond360);
            
        yield return new WaitForSecondsRealtime(4f);
        
        
        // 오브젝트 꺼주기
    }
    
    private IEnumerator Co_Mix(Transform main, Transform target)
    {
        var targetPos = target.position;
        
        main.LookAt(target);

        // 이동
        var firstPos = main.position;
        var secondPos = (firstPos + targetPos) / 2f + new Vector3(0f, 0.2f, 0f);
        var thirdPos = targetPos + new Vector3(0f, 0.14f, 0f);
        
        main.DOPath(new[] {firstPos, secondPos, thirdPos}, 3f,PathType.CatmullRom).SetEase(Ease.InQuad);
            
        yield return new WaitForSecondsRealtime(4f);

        // 행동
        
        main.DORotate(main.rotation.eulerAngles + new Vector3(0f, 1440f, 0f), 4f, RotateMode.FastBeyond360);
            
        yield return new WaitForSecondsRealtime(4.5f);
        
        
        // 오브젝트 꺼주기
    }
    
    private IEnumerator Co_Cut(Transform main, Transform target)
    {
        var targetPos = target.position;
        
        main.LookAt(target);
        target.LookAt(main);

        // 이동
        var firstPos = main.position;
        var secondPos = (firstPos + targetPos) / 2f + new Vector3(0f, 0.2f, 0f);
        var thirdPos = targetPos + new Vector3(0f, 0.14f, 0f);
        
        main.DOPath(new[] {firstPos, secondPos, thirdPos}, 3f,PathType.CatmullRom).SetEase(Ease.InQuad);
            
        yield return new WaitForSecondsRealtime(4f);

        // 행동
        main.DOPath(new[] {thirdPos , targetPos, thirdPos, targetPos, thirdPos, targetPos, thirdPos, targetPos, thirdPos, thirdPos}, 3f,PathType.Linear).SetEase(Ease.Linear);
        main.Rotate(0f, -30f, 0f);
        main.DORotate(main.rotation.eulerAngles + new Vector3(0f, 60f, 0f), 3.5f, RotateMode.FastBeyond360);
            
        yield return new WaitForSecondsRealtime(4.5f);
        
        
        // 오브젝트 꺼주기
    }
    
}