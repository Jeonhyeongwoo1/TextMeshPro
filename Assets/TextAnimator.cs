using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;
using System.Linq;


public class TextAnimator : MonoBehaviour
{
    public TextAnimatorData animatorData;
    private bool m_Direction = false;
    private int m_TextCharacterCount = 0;
    private float ratio = 0;

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

    float GetDuration(TextAnimatorData textAnimatorData)
    {
        float color = textAnimatorData.charColorInfo.use ? textAnimatorData.charColorInfo.duration : 0;
        float scale = textAnimatorData.scaleInfo.use ? textAnimatorData.scaleInfo.duration : 0;
        float position = textAnimatorData.positionInfo.use ? textAnimatorData.positionInfo.duration : 0;
        float rotation = textAnimatorData.rotationInfo.use ? textAnimatorData.rotationInfo.duration : 0;

        float duration = Mathf.Max(color,
                                    Mathf.Max(scale,
                                    Mathf.Max(position, rotation)));
        return duration;
    }

    IEnumerator ChangingColor(UnityAction done = null)
    {
        Color color;
        float duration = animatorData.colorInfo.duration;
        float elapsed = 0;
        TextAnimatorData.ItemColor<Color> itmeColor = animatorData.colorInfo;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color = Color.LerpUnclamped(animatorData.textmesh.color, itmeColor.color, itmeColor.curve.Evaluate(elapsed / duration));
            animatorData.textmesh.color = color;
            yield return null;
        }

        yield return null;

        done?.Invoke();
    }

    IEnumerator CharacterProceeding(TMP_TextInfo textInfo, TMP_MeshInfo[] vertextMeshInfoData, int index, UnityAction done = null)
    {
        Matrix4x4 matrix = default;
        Color32[] vertexColor = default;
        float elapsed = 0;
        float d = GetDuration(animatorData);

        while (elapsed < d)
        {
            elapsed += Time.deltaTime;

            int materialIndex = textInfo.characterInfo[index].materialReferenceIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
            Vector3[] sourceVertices = vertextMeshInfoData[materialIndex].vertices;
            int vertexIndex = textInfo.characterInfo[index].vertexIndex;
            Vector3 offset = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;
            Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;
            Vector3 rotation = Vector3.zero, position = Vector3.zero, scale = Vector3.one;

            if (animatorData.charColorInfo.use)
            {
                vertexColor = textInfo.meshInfo[materialIndex].colors32;
                float duration = animatorData.charColorInfo.duration;
                Color color = animatorData.charColorInfo.gradient.Evaluate(elapsed / duration);

                for (int j = 0; j < 4; j++)
                    vertexColor[vertexIndex + j] = color;

            }

            if (animatorData.positionInfo.use)
            {
                float duration = animatorData.positionInfo.duration;
                Vector3 from = animatorData.positionInfo.from;
                Vector3 to = animatorData.positionInfo.to;

                position = Vector3.LerpUnclamped(from, to, animatorData.positionInfo.curve.Evaluate(elapsed / duration));
            }

            if (animatorData.rotationInfo.use)
            {
                float duration = animatorData.rotationInfo.duration;
                Vector3 from = animatorData.rotationInfo.from;
                Vector3 to = animatorData.rotationInfo.to;

                rotation = Vector3.LerpUnclamped(from, to, animatorData.rotationInfo.curve.Evaluate(elapsed / duration));
            }

            if (animatorData.scaleInfo.use)
            {
                float duration = animatorData.scaleInfo.duration;
                Vector3 from = animatorData.scaleInfo.from;
                Vector3 to = animatorData.scaleInfo.to;

                scale = Vector3.LerpUnclamped(from, to, animatorData.scaleInfo.curve.Evaluate(elapsed / duration));
            }

            matrix = Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale);

            for (int j = 0; j < 4; j++)
                destinationVertices[vertexIndex + j] = matrix.MultiplyPoint(sourceVertices[vertexIndex + j] - offset) + offset;

            for (int j = 0; j < textInfo.meshInfo.Length; j++)
            {
                textInfo.meshInfo[j].mesh.vertices = textInfo.meshInfo[j].vertices;
                textInfo.meshInfo[j].mesh.colors32 = textInfo.meshInfo[j].colors32;
                textInfo.textComponent.UpdateGeometry(textInfo.meshInfo[j].mesh, j);
            }

            yield return null;
        }

        done?.Invoke();
    }

    IEnumerator ElapsedProgress()
    {
        float elapsed = 0, duration = GetDuration(animatorData);
        bool direction = animatorData.progress < 1 ? true : false;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float value = Mathf.Clamp01(Mathf.Max((elapsed / duration), ratio));
            animatorData.progress = value;
            yield return null;
        }
    }

    IEnumerator RandomProceeding()
    {
        yield return new WaitForSeconds(2f);



        yield return null;
    }

    public List<int> Shuffle(int count)
    {
        List<int> list = new List<int>();
        System.Random rng = new System.Random();

        for (int i = 0; i < count; i++) { list.Add(i); }

        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }

    IEnumerator Proceeding(UnityAction done = null)
    {
        yield return new WaitForSeconds(2f);

        if (animatorData == null) { yield break; }

        do
        {
            int count = 0;
            bool shuffle = animatorData.shuffle;
            float delay = animatorData.characterDelay;
            TMP_TextInfo textInfo = animatorData.textmesh.textInfo;
            TMP_MeshInfo[] vertextMeshInfoData = textInfo.CopyMeshInfoVertexData();
            int characterCount = textInfo.characterCount;
            m_Direction = animatorData.progress < 1 ? true : false;
            ratio = 0;

            List<int> list = new List<int>();

            if (shuffle)
            {
                list = Shuffle(animatorData.textmesh.textInfo.characterCount);
            }

            if (animatorData.useMaxVisibleCharacter)
            {
                textInfo.textComponent.maxVisibleCharacters = 0;
            }

            StartCoroutine(ElapsedProgress());

            for (int i = 0; i < characterCount; i++)
            {
                int index = shuffle ? list[i] : i;
                if (textInfo.characterInfo[index].character == ' ') { count++; continue; }

                if (animatorData.sequence)
                {
                    yield return CharacterProceeding(textInfo, vertextMeshInfoData, index, () =>
                                {
                                    count++;
                                    ratio = Mathf.Clamp01((float)count / (float)textInfo.characterCount);
                                });
                }
                else
                {
                    StartCoroutine(CharacterProceeding(textInfo, vertextMeshInfoData, index, () =>
                     {
                         count++;
                         ratio = Mathf.Clamp01((float)count / (float)textInfo.characterCount);
                     }));
                }

                yield return new WaitForSeconds(delay);
            }

            while (count != textInfo.characterCount) { yield return null; }

            yield return new WaitForSeconds(animatorData.waitingTimeAfterLooping);

            if (animatorData.useMaxVisibleCharacter)
            {
                textInfo.textComponent.maxVisibleCharacters = m_TextCharacterCount;
            }

        } while (animatorData.isLooping);

    }

    void Start()
    {
        if (animatorData != null)
        {
            if (animatorData.textmesh == null)
            {
                animatorData.textmesh = GetComponent<TMP_Text>();
            }
        }

        StartAnimation();


    }

    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        m_TextCharacterCount = animatorData.textmesh.GetTextInfo(animatorData.textmesh.text).characterCount;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        animatorData.textmesh.GetTextInfo(animatorData.textmesh.text).characterCount = m_TextCharacterCount;
    }



}
