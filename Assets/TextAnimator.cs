using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TextAnimator : MonoBehaviour
{
    [TextArea]
    public string customText;
    public TextMeshProUGUI textMeshPro;
    public float shakeAmount = 1f;
    public bool shake = false;

    public float waveAmount = 1f;
    public float waveSpeed = 1f;
    public float waveSeparation = 1f;
    public bool wave = false;
    public bool waveCharactor = false;

    public float jumpingAmount = 1f;
    public float jumpingSpeed = 1f;
    public float jumpingEnd = 1f;
    public AnimationCurve curve;

    private Vector3[][] vertex_Base;
    private List<bool> enableShaking = new List<bool>();
    private float curFrame = 0f;



    //SHAKE
    Vector3 ShakeVector(float amount)
    {
        return new Vector3(Random.Range(-amount, amount), Random.Range(-amount, amount));
    }

    Vector3 JumpingVector(float amout)
    {
        return new Vector3(amout, amout);
    }

    //WAVE
    Vector3 WaveVector(float amount, float time)
    {
        return new Vector3(0, Mathf.Cos(time) * amount);
    }

    //<t> </t>
    string ParseText(string inputText)
    {
        string text = "";
        

        if(inputText.Contains("<t>"))
        {
            string startTag = "<t>";
            string endTag = "</t>";
        
            int startIndex = inputText.IndexOf(startTag);  
            int lastIndex = inputText.IndexOf(endTag);

            

            for (int i = 0; i < inputText.Length; i++)
            {
                if(i >= startIndex + 3 || i < lastIndex)
                {
                    enableShaking.Add(true);
                }
                else
                {
                    enableShaking.Add(false);
                }

                
            }


        }



        if(inputText.Contains("<t>"))
        {
            string startTag = "<t>";
            string endTag = "</t>";
        
            int startIndex = inputText.IndexOf(startTag);  
            int lastIndex = inputText.IndexOf(endTag);

            for (int i = startIndex + 3; i < lastIndex; i++)
            {
                text += inputText[i];
            }

            
            

        }
    
        return text;
    } 

    public void UpdateText(string textMeshPro)
    {
        string animationTxt = ParseText(textMeshPro);
        
        

    }

    public void SyncToMesh()
    {
        textMeshPro.ForceMeshUpdate();
        vertex_Base = new Vector3[textMeshPro.textInfo.meshInfo.Length][];

        for (int i = 0; i < textMeshPro.textInfo.meshInfo.Length; ++i)
        {
            vertex_Base[i] = new Vector3[textMeshPro.textInfo.meshInfo[i].vertices.Length];
            System.Array.Copy(textMeshPro.textInfo.meshInfo[i].vertices, vertex_Base[i], textMeshPro.textInfo.meshInfo[i].vertices.Length);
        }

        textMeshPro.ForceMeshUpdate();
    }

    IEnumerator CharactorJumping()
    {
        Matrix4x4 matrix;
        int count = textMeshPro.textInfo.characterCount;
        float duration = 1f;
        float elapsed = 0;
        float length = 1f;
   
        while(true)
        {
            int loopCount = 0;
            
            int random = Random.Range(0, count);
            int c = textMeshPro.textInfo.meshInfo.Length;
            int r = Random.Range(0, c);
            int vertexIndex = textMeshPro.textInfo.characterInfo[random].vertexIndex;
            Vector3 jumpingOffset;

            for (byte k = 0; k < 4; k++)
            {
                textMeshPro.textInfo.meshInfo[r].vertices[vertexIndex + k] = vertex_Base[r][vertexIndex + k];
            }

            textMeshPro.UpdateVertexData();

            while(elapsed < duration)
            {
                elapsed += Time.deltaTime;
                jumpingOffset = new Vector3(0, Mathf.Lerp(0, jumpingEnd, elapsed / duration));
                matrix = Matrix4x4.TRS(jumpingOffset * jumpingAmount, Quaternion.identity, Vector3.one);

                for (byte k = 0; k < 4; k++)
                {
                    textMeshPro.textInfo.meshInfo[r].vertices[vertexIndex + k] = matrix.MultiplyPoint3x4(textMeshPro.textInfo.meshInfo[r].vertices[vertexIndex + k]);
                }

                textMeshPro.UpdateVertexData();

                yield return null;
            }

            elapsed = 0;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                jumpingOffset = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0);
                matrix = Matrix4x4.TRS(jumpingOffset * 4f, Quaternion.identity, Vector3.one);

                for (byte k = 0; k < 4; k++)
                {
                    textMeshPro.textInfo.meshInfo[r].vertices[vertexIndex + k] = matrix.MultiplyPoint3x4(textMeshPro.textInfo.meshInfo[r].vertices[vertexIndex + k]);
                }

                textMeshPro.UpdateVertexData();
                yield return null;
            }

            matrix = Matrix4x4.TRS(Vector3.one, Quaternion.identity, Vector3.one);

            for (byte k = 0; k < 4; k++)
            {
                textMeshPro.textInfo.meshInfo[r].vertices[vertexIndex + k] = matrix.MultiplyPoint3x4(textMeshPro.textInfo.meshInfo[r].vertices[vertexIndex + k]);
            }

            elapsed = 0;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                jumpingOffset = new Vector3(0, Mathf.Lerp(jumpingEnd, 0, elapsed / duration));
                matrix = Matrix4x4.TRS(jumpingOffset * jumpingAmount, Quaternion.identity, Vector3.one);
                matrix = matrix.inverse;
                
                for (byte k = 0; k < 4; k++)
                {
                    textMeshPro.textInfo.meshInfo[r].vertices[vertexIndex + k] = matrix.MultiplyPoint3x4(textMeshPro.textInfo.meshInfo[r].vertices[vertexIndex + k]);
                }

                textMeshPro.UpdateVertexData();

                yield return null;
            }

            // while (loopCount < count)
            // {

            //     Vector3 jumpingOffset = new Vector3(0, Mathf.PingPong(loopCount / 25f * jumpingSpeed, 1));

            //     matrix = Matrix4x4.TRS(jumpingOffset * jumpingAmount, Quaternion.identity, Vector3.one);
            //     print(jumpingOffset * jumpingAmount);
               
            //     for (byte k = 0; k < 4; k++)
            //     {
            //         textMeshPro.textInfo.meshInfo[r].vertices[vertexIndex + k] = matrix.MultiplyPoint3x4(textMeshPro.textInfo.meshInfo[r].vertices[vertexIndex + k]);

            //     }

            //     textMeshPro.UpdateVertexData();

            //     yield return new WaitForSeconds(0.1f);

            //     loopCount++;
            // }
        }
       

      
    }


    // // // Update is called once per frame
    // void Update()
    // {

    //     Vector3 sv = new Vector3();
    //     Vector3 wv = new Vector3();

    //     for (int i = 0; i < textMeshPro.textInfo.meshInfo.Length; i++)
    //     {
    //         int j = 0;

    //         for (int v = 0; v < textMeshPro.textInfo.meshInfo[i].vertices.Length; v += 4, ++j)
    //         {

    //             for (byte k = 0; k < 4; k++)
    //             {
    //                 textMeshPro.textInfo.meshInfo[i].vertices[v + k] = vertex_Base[i][v + k];
    //             }

    //             if (shake)
    //             {
    //                 for (byte k = 0; k < 4; k++)
    //                 {
    //                     sv = ShakeVector(shakeAmount);
    //                     textMeshPro.textInfo.meshInfo[i].vertices[v + k] += sv;
    //                 }
    //             }

    //             if (wave)
    //             {
    //                 if (waveCharactor)
    //                 {
    //                     wv = WaveVector(waveAmount, curFrame * waveSpeed + textMeshPro.textInfo.meshInfo[i].vertices[v].x / waveSeparation);
    //                 }

    //                 for (byte k = 0; k < 4; k++)
    //                 {
    //                     if (!waveCharactor)
    //                     {
    //                         wv = WaveVector(waveAmount, curFrame * waveSpeed);
    //                     }

    //                     textMeshPro.textInfo.meshInfo[i].vertices[v + k] += wv;
    //                 }
    //             }

    //         }

    //         textMeshPro.UpdateVertexData();
    //     }

    //     curFrame++;
    // }

    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        SyncToMesh();
        UpdateText(textMeshPro.text);
        StartCoroutine(CharactorJumping());
    }

}
