using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SubTitle : MonoBehaviour
{
    public TextMeshProUGUI mainTitle;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public TMP_TextInfo textInfo;
    public AnimationCurve scaleCurve;
    public AnimationCurve rotationCurve;
    public AnimationCurve posCurve;
    public Gradient ColorGradient;

    public float speed;
    public float posMultiplier;
    public Vector3 rotation;
    public Vector3 position;
    public float charWaitTime = 0.05f;
    public float maxScale = 1.1f;

    private float time = 0f;


    // Start is called before the first frame update
    void Start()
    {
        textInfo = mainTitle.GetTextInfo(mainTitle.text);
       // StartCoroutine(RadomScale());
        OpeningAnimation();
    }

    public void OpeningAnimation()
    {
        gameObject.SetActive(true);

        StartCoroutine(Animation3());

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Animation3()
    {

        while (true)
        {
            TMP_MeshInfo[] cachedMeshInfoVertexData = textInfo.CopyMeshInfoVertexData();
            textInfo.textComponent.maxVisibleCharacters = 0;
            int count = 0;
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (textInfo.characterInfo[i].character == ' ') { count++; continue; }
                StartCoroutine(CharAnimation(i, cachedMeshInfoVertexData, count));
                count++;
                yield return new WaitForSeconds(charWaitTime);
            }

            yield return new WaitForSeconds(2);
        }
    }

    IEnumerator RadomScale()
    {
        TMP_MeshInfo[] cachedMeshInfoVertexData = textInfo.CopyMeshInfoVertexData();
        Color32[] vertexColor;

        while(true)
        {
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                float time = 0;
                StartCoroutine(CharRamdomScale(i, cachedMeshInfoVertexData));
                yield return null;
            }

        }
       

    }

    IEnumerator CharRamdomScale(int i, TMP_MeshInfo[] cachedMeshInfoVertexData)
    {

        int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
        Matrix4x4 matrix;
        Vector3 scale = new Vector3(UnityEngine.Random.Range(1f, maxScale), UnityEngine.Random.Range(1f, maxScale));
        Vector3 curScale = Vector3.zero;
        
        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
        Vector3[] sourceVertices = cachedMeshInfoVertexData[materialIndex].vertices;
        int vertexIndex = textInfo.characterInfo[i].vertexIndex;
        Vector3 offset = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;
        Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

        do
        {
            time += Time.deltaTime;
            curScale = Vector3.Lerp(Vector3.one, scale, time);
            matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, curScale);

            destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(sourceVertices[vertexIndex + 0] - offset) + offset;
            destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(sourceVertices[vertexIndex + 1] - offset) + offset;
            destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(sourceVertices[vertexIndex + 2] - offset) + offset;
            destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(sourceVertices[vertexIndex + 3] - offset) + offset;

            for (int j = 0; j < textInfo.meshInfo.Length; j++)
            {
                textInfo.meshInfo[j].mesh.vertices = textInfo.meshInfo[j].vertices;
                textInfo.meshInfo[j].mesh.colors32 = textInfo.meshInfo[j].colors32;
                textInfo.textComponent.UpdateGeometry(textInfo.meshInfo[j].mesh, j);
            }
            
            yield return null;
        }
        while (scale != curScale);
        
    }

    IEnumerator CharAnimation(int i, TMP_MeshInfo[] cachedMeshInfoVertexData, int count)
    {
        float time = 0, posElapsed = 0;
        Vector3 scale = Vector3.zero, rotation = Vector3.zero, pos = Vector3.zero;
        Matrix4x4 matrix4X4;
        Color32[] vertexColor;
        Color32 c0 = textInfo.textComponent.color;
        
        int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

        do
        {

            time += Time.deltaTime;
            posElapsed += Time.deltaTime;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
            Vector3[] sourceVertices = cachedMeshInfoVertexData[materialIndex].vertices;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Vector3 offset = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;
            Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;
            float alpha = Mathf.Lerp(0, 1, curve.Evaluate(time / 1f));

            pos = Vector3.LerpUnclamped(Vector3.zero, position, posCurve.Evaluate(posElapsed / 1f));
            rotation = Vector3.LerpUnclamped(Vector3.zero, this.rotation, rotationCurve.Evaluate(time / 1f));
            scale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, scaleCurve.Evaluate(time / 1));

            matrix4X4 = Matrix4x4.TRS(pos, Quaternion.Euler(rotation), scale);
            //matrix4X4 = Matrix4x4.Rotate(Qu aternion.Euler(rotation));
            destinationVertices[vertexIndex + 0] = matrix4X4.MultiplyPoint3x4(sourceVertices[vertexIndex + 0] - offset) + offset;
            destinationVertices[vertexIndex + 1] = matrix4X4.MultiplyPoint3x4(sourceVertices[vertexIndex + 1] - offset) + offset;
            destinationVertices[vertexIndex + 2] = matrix4X4.MultiplyPoint3x4(sourceVertices[vertexIndex + 2] - offset) + offset;
            destinationVertices[vertexIndex + 3] = matrix4X4.MultiplyPoint3x4(sourceVertices[vertexIndex + 3] - offset) + offset;
    
            vertexColor = textInfo.meshInfo[materialIndex].colors32;
            c0 = ColorGradient.Evaluate(time);

            vertexColor[vertexIndex + 0] = c0;
            vertexColor[vertexIndex + 1] = c0;
            vertexColor[vertexIndex + 2] = c0;
            vertexColor[vertexIndex + 3] = c0;
            
            // Push changes into meshes
            for (int j = 0; j < textInfo.meshInfo.Length; j++)
            {
                textInfo.meshInfo[j].mesh.vertices = textInfo.meshInfo[j].vertices;
                textInfo.meshInfo[j].mesh.colors32 = textInfo.meshInfo[j].colors32;
                textInfo.textComponent.UpdateGeometry(textInfo.meshInfo[j].mesh, j);
            }

            yield return null;
        } while (pos != Vector3.zero);

        textInfo.textComponent.maxVisibleCharacters = count;

    }
}