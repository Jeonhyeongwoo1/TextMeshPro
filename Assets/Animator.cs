using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Animator : MonoBehaviour
{
    public TextMeshProUGUI mainTitle;
    public ParticleSystem.MinMaxGradient colors;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float multiplier;

    void Start()
    {
        StartCoroutine(RevealScale());
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
    }

    IEnumerator Animation()
    {
        yield return RevealText();
        yield return ChangeColor();
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

        while (true)
        {
            if (visibleCount == totalCharCount + 1) { break; }

            textInfo.textComponent.maxVisibleCharacters = visibleCount;
            visibleCount++;
            
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (textInfo.characterInfo[i].character == ' ') { continue; }

                float scale = 1.5f;

                while(scale > 1f)
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

                    float randomScale = Random.Range(1, 1.5f);

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
                    Color color = colors.Evaluate(Time.deltaTime * multiplier);
                    c0 = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);

                    vertexColor[vertexIndex + 0] = color;
                    vertexColor[vertexIndex + 1] = color;
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

                   
                   // mainTitle.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);


                    scale -= Time.deltaTime;
                    yield return null;
                }

              

            }

         

            yield return new WaitForSeconds(0.1f);
        }

    }


    IEnumerator ChangeColor()
    {
        TMP_TextInfo textInfo = mainTitle.GetTextInfo(mainTitle.text);
        Color32[] vertexColor;
        Color32 c0 = textInfo.textComponent.color;
        TMP_CharacterInfo[] characterInfos = textInfo.characterInfo;


        while (true)
        {

            for (int i = 0; i < textInfo.characterCount; i++)
            {

                TMP_MeshInfo[] meshInfos = textInfo.meshInfo;
                int vertexIndex = characterInfos[i].vertexIndex;
                int materalIndex = characterInfos[i].materialReferenceIndex;
                vertexColor = meshInfos[materalIndex].colors32;
                Color color = colors.Evaluate(Time.deltaTime * multiplier);
                c0 = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);

                vertexColor[vertexIndex + 0] = color;
                vertexColor[vertexIndex + 1] = color;
                vertexColor[vertexIndex + 2] = c0;
                vertexColor[vertexIndex + 3] = c0;
                meshInfos[materalIndex].colors32 = vertexColor;

                mainTitle.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                yield return new WaitForSeconds(0.01f);
            }

        }

    }

}