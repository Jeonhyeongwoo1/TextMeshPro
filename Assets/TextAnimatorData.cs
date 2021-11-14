using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[Serializable]
public class TextAnimatorData
{

    [Serializable]
    public class BaseItem
    {
        public bool use;
        public AnimationCurve curve;
        public float duration;
    }

    [Serializable]
    public class ItemFromTo<T> : BaseItem
    {
        public T from;
        public T to;
    }

    [Serializable]
    public class ItemGradient<T> : BaseItem where T : Gradient
    {
        public Gradient gradient;
    }

    [Serializable]
    public class ItemFloatTo<T> : BaseItem
    {
        public T from;
        public T to;
    }

    [Serializable]
    public class ItemColor<T> : BaseItem
    {
        public Color color;
    }

    public TMP_Text textmesh;

    [Range(0, 1)] public float progress = 0f;
    public ItemColor<Color> colorInfo; //change full color  (vertex Color)
    public ItemGradient<Gradient> charColorInfo; //change character color  (character vertex color)
    public ItemFromTo<Vector3> scaleInfo;
    public ItemFromTo<Vector3> positionInfo;
    public ItemFromTo<Vector3> rotationInfo;

    public bool isLooping = false;
    public bool sequence = false;
    public bool useMaxVisibleCharacter = false;
    public bool shuffle = false;

    public float waitingTimeAfterLooping;
    public float characterDelay;
}
