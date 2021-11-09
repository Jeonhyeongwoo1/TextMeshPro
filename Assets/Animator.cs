using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Animator : MonoBehaviour
{
    public TextMeshProUGUI mainTitle;
    public Gradient colors;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float angleSpeed;
    public float scaleSpeed;
    public float colorMultiplier;
    public float AngleMultiplier;
    public float colorSpeed = 0.01f;
    public float speed;
    public float axisX;
    public float movingMultiplier;

    void Start()
    {
        StartCoroutine(Animation3());
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
    }

    IEnumerator Animation()
    {
        yield return new WaitForSeconds(1f);
        yield return RevealScale();
        yield return ChangeColor();
    }

    IEnumerator Animation3()
    {
        Matrix4x4 matrix;

        TMP_TextInfo textInfo = mainTitle.GetTextInfo(mainTitle.text);


        for(int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_MeshInfo[] copyMeshInfo = textInfo.CopyMeshInfoVertexData();
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Vector3[] sourceVertices = copyMeshInfo[materialIndex].vertices;
            Vector2 charMidBaseline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

            Vector3 offset = charMidBaseline;
            Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

            destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
            destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
            destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
            destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

            matrix = Matrix4x4.TRS(Vector3.left * movingMultiplier, Quaternion.identity, Vector3.one);

            destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
            destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
            destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
            destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

            destinationVertices[vertexIndex + 0] += offset;
            destinationVertices[vertexIndex + 1] += offset;
            destinationVertices[vertexIndex + 2] += offset;
            destinationVertices[vertexIndex + 3] += offset;

        }

        while (true)
        {
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (textInfo.characterInfo[i].character == ' ') { continue; }

                TMP_MeshInfo[] originMeshInfo = textInfo.CopyMeshInfoVertexData();

                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                TMP_MeshInfo[] copyMeshInfo = textInfo.CopyMeshInfoVertexData();
                Vector3[] sourceVertices = copyMeshInfo[materialIndex].vertices;
                Vector2 charMidBaseline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                Vector3 offset = charMidBaseline;
                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                matrix = Matrix4x4.TRS(Vector3.left * movingMultiplier, Quaternion.identity, Vector3.one);

                destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                destinationVertices[vertexIndex + 0] += offset;
                destinationVertices[vertexIndex + 1] += offset;
                destinationVertices[vertexIndex + 2] += offset;
                destinationVertices[vertexIndex + 3] += offset;

                for (int j = 0; j < textInfo.meshInfo.Length; j++)
                {
                    textInfo.meshInfo[j].mesh.vertices = textInfo.meshInfo[j].vertices;
                    textInfo.textComponent.UpdateGeometry(textInfo.meshInfo[j].mesh, j);
                }

                yield return null;


            }
        }
        
    }

    //두바퀴 턴
    IEnumerator RotationAnimation()
    {
        Matrix4x4 matrix;

        TMP_TextInfo textInfo = mainTitle.GetTextInfo(mainTitle.text);


        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (textInfo.characterInfo[i].character == ' ') { continue; }

            int count = 0;
            int materialCount = textInfo.materialCount;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            TMP_MeshInfo[] meshInfo = textInfo.CopyMeshInfoVertexData();
            Vector3[] orginVerteices = meshInfo[materialIndex].vertices;
            double v0 = Math.Round(orginVerteices[vertexIndex + 0].y, 1);
            double v1 = Math.Round(orginVerteices[vertexIndex + 1].y, 1);

            float mid = (Mathf.Abs((float)v1) - Mathf.Abs((float)v0)) / 2;

            do
            {
                TMP_MeshInfo[] copyMeshInfo = textInfo.CopyMeshInfoVertexData();
                TMP_MeshInfo[] meshInfos = textInfo.meshInfo;
                Vector3[] sourceVertices = copyMeshInfo[materialIndex].vertices;
                Vector2 charMidBaseline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                Vector3 offset = charMidBaseline;
                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(angleSpeed, 0, 0), Vector3.one);

                destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                destinationVertices[vertexIndex + 0] += offset;
                destinationVertices[vertexIndex + 1] += offset;
                destinationVertices[vertexIndex + 2] += offset;
                destinationVertices[vertexIndex + 3] += offset;

                for (int j = 0; j < textInfo.meshInfo.Length; j++)
                {
                    textInfo.meshInfo[j].mesh.vertices = textInfo.meshInfo[j].vertices;
                    textInfo.textComponent.UpdateGeometry(textInfo.meshInfo[j].mesh, j);

                }

                double value = Math.Round(destinationVertices[vertexIndex + 0].y, 1);

                if (mid < value)
                {
                    if (Mathf.Approximately((float)value, (float)v1))
                    {
                        count++;
                    }
                }

                if (count == 1 && Mathf.Approximately((float)value, (float)v0))
                {
                    count++;
                }


                yield return null;
            } while (count != 2);

        }
    }

    IEnumerator RevealText()
    {
        TMP_TextInfo textInfo = mainTitle.GetTextInfo(mainTitle.text);
        int totalCharCount = textInfo.characterCount;
        int visibleCount = 0;

        while (true)
        {
            if (visibleCount == totalCharCount + 1) { break; }

            textInfo.textComponent.maxVisibleCharacters = visibleCount;
            visibleCount++;
            yield return new WaitForSeconds(0.05f);
        }

    }

    IEnumerator RevealScale()
    {
        TMP_TextInfo textInfo = mainTitle.GetTextInfo(mainTitle.text);
        int totalCharCount = textInfo.characterCount;
        int visibleCount = 0;
        Matrix4x4 matrix;
        TMP_MeshInfo[] cachedMeshInfoVertexData = textInfo.CopyMeshInfoVertexData();
        Color32[] vertexColor;
        Color32 c0 = textInfo.textComponent.color;
        int count = 0;
        float totalTime = 0;

        while (visibleCount != totalCharCount + 1)
        {
            textInfo.textComponent.maxVisibleCharacters = visibleCount;
            visibleCount++;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (textInfo.characterInfo[i].character == ' ') { continue; }

                float scale = 2f;

                while (scale > 1f)
                {
                    int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                    Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
                    Vector3[] sourceVertices = cachedMeshInfoVertexData[materialIndex].vertices;

                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                    Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                    // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
                    // This is needed so the matrix TRS is applied at the origin for each character.
                    Vector3 offset = charMidBasline;

                    Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                    destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                    destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                    destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                    destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                    matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * scale);
                    vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
                    vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
                    vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
                    vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);

                    destinationVertices[vertexIndex + 0] += offset;
                    destinationVertices[vertexIndex + 1] += offset;
                    destinationVertices[vertexIndex + 2] += offset;
                    destinationVertices[vertexIndex + 3] += offset;

                    Vector2[] sourceUVs0 = cachedMeshInfoVertexData[materialIndex].uvs0;
                    Vector2[] destinationUVs0 = textInfo.meshInfo[materialIndex].uvs0;

                    destinationUVs0[vertexIndex + 0] = sourceUVs0[vertexIndex + 0];
                    destinationUVs0[vertexIndex + 1] = sourceUVs0[vertexIndex + 1];
                    destinationUVs0[vertexIndex + 2] = sourceUVs0[vertexIndex + 2];
                    destinationUVs0[vertexIndex + 3] = sourceUVs0[vertexIndex + 3];

                    TMP_MeshInfo[] meshInfos = textInfo.meshInfo;
                    vertexColor = meshInfos[materialIndex].colors32;

                    float colorOffset = (i / textInfo.characterCount);
                    c0 = colors.Evaluate((totalTime + colorOffset) % 1);
                    totalTime += Time.deltaTime;

                    vertexColor[vertexIndex + 0] = c0;
                    vertexColor[vertexIndex + 1] = c0;
                    vertexColor[vertexIndex + 2] = c0;
                    vertexColor[vertexIndex + 3] = c0;
                    meshInfos[materialIndex].colors32 = vertexColor;

                    // Push changes into meshes
                    for (int j = 0; j < textInfo.meshInfo.Length; j++)
                    {

                        // Updated modified vertex attributes
                        textInfo.meshInfo[j].mesh.vertices = textInfo.meshInfo[j].vertices;
                        textInfo.meshInfo[j].mesh.uv = textInfo.meshInfo[j].uvs0;
                        textInfo.meshInfo[j].mesh.colors32 = textInfo.meshInfo[j].colors32;

                        textInfo.textComponent.UpdateGeometry(textInfo.meshInfo[j].mesh, j);
                    }

                    scale -= Time.deltaTime * scaleSpeed;
                    yield return null;
                }

                TMP_CharacterInfo[] infos = textInfo.characterInfo;


                for (int j = 0; j < textInfo.meshInfo.Length; j++)
                {
                    TMP_MeshInfo[] meshInfos = textInfo.meshInfo;
                    int vertexIndex = infos[i].vertexIndex;
                    int materalIndex = infos[i].materialReferenceIndex;
                    vertexColor = meshInfos[materalIndex].colors32;

                    vertexColor[vertexIndex + 0] = Color.black;
                    vertexColor[vertexIndex + 1] = Color.black;
                    vertexColor[vertexIndex + 2] = Color.black;
                    vertexColor[vertexIndex + 3] = Color.black;
                    meshInfos[materalIndex].colors32 = vertexColor;

                    mainTitle.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                    yield return new WaitForSeconds(0.01f);
                }

            }
            TMP_CharacterInfo[] characterInfos = textInfo.characterInfo;

            totalTime = 0;
            while (true)
            {

                for (int i = 0; i < textInfo.characterCount; i++)
                {

                    TMP_MeshInfo[] meshInfos = textInfo.meshInfo;
                    int vertexIndex = characterInfos[i].vertexIndex;
                    int materalIndex = characterInfos[i].materialReferenceIndex;
                    vertexColor = meshInfos[materalIndex].colors32;
                    float offset = (i / textInfo.characterCount);

                    c0 = colors.Evaluate((totalTime + offset) % 1);
                    totalTime += Time.deltaTime;
                    vertexColor[vertexIndex + 0] = c0;
                    vertexColor[vertexIndex + 1] = c0;
                    vertexColor[vertexIndex + 2] = c0;
                    vertexColor[vertexIndex + 3] = c0;
                    meshInfos[materalIndex].colors32 = vertexColor;

                    mainTitle.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                    yield return new WaitForSeconds(colorSpeed);
                }

            }
        }

    }


    IEnumerator ChangeColor()
    {
        TMP_TextInfo textInfo = mainTitle.GetTextInfo(mainTitle.text);
        textInfo.textComponent.ForceMeshUpdate();
        Color32[] vertexColor;
        Color32 c0 = textInfo.textComponent.color;
        TMP_CharacterInfo[] characterInfos = textInfo.characterInfo;
        float totalTime = 0;
        while (true)
        {

            for (int i = 0; i < textInfo.characterCount; i++)
            {

                TMP_MeshInfo[] meshInfos = textInfo.meshInfo;
                int vertexIndex = characterInfos[i].vertexIndex;
                int materalIndex = characterInfos[i].materialReferenceIndex;
                vertexColor = meshInfos[materalIndex].colors32;
                float offset = (i / textInfo.characterCount);

                c0 = colors.Evaluate((totalTime + offset) % 1);
                totalTime += Time.deltaTime;
                vertexColor[vertexIndex + 0] = c0;
                vertexColor[vertexIndex + 1] = c0;
                vertexColor[vertexIndex + 2] = c0;
                vertexColor[vertexIndex + 3] = c0;
                meshInfos[materalIndex].colors32 = vertexColor;

                mainTitle.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                yield return new WaitForSeconds(colorSpeed);
            }

        }

    }

}