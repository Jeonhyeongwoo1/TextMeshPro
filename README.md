# TextMeshPro Animation Samples

텍스트 메쉬 프로를 이용하여 텍스트 애니메이션 예제 샘플 프로젝트입니다.

## Version 

 * Unity Version : 2020.3.11f1
 * TextMeshpro : 3.06


## Component

 * 코루틴을 사용하여 애니메이션 효과 구현
 * Text Character를 이용하여 애니메이션 효과 구현
 
 Variable | Description
 ---|---|
 colorInfo | TextMeshPro Vertex Color
 charColorInfo | TextMeshPro Vertex Character Color
 scaleInfo | TextMeshPro Vertex Character Scale 
 positionInfo | TextMeshPro Vertex Character Position
 rotationInfo | TextMeshPro Vertex Character Rotation
 isLooping | Whether to play repeatedly
 sequence | Whether to play sequence
 useMaxVisibleCharacter | Whether to play visible Character
 shuffle | Whether to play random
 waitingTimeAfterLooping | next Animation WaitTime
 characterDelay | character Animation Delay
 
## Code
 * Public Method

<pre>
<code>
    public void StartAnimation()
    {
        if (animatorData.colorInfo.use)
        {
            StartCoroutine(ChangingColor());
        }
        else
        {
            StartCoroutine(Proceeding());
        }
    }
</code>
</pre>
 
 ## Inspector
 
 ![image](https://user-images.githubusercontent.com/50667930/142714814-0d44cde3-d472-48d0-a73e-caf1d9a83904.png)

 
 
 ## 참고 자료
 https://github.com/coposuke/TextMeshProAnimator
