using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class TextEffect : MonoBehaviour
{
    [SerializeField] protected TMP_Text textComponent;

    private float floatSpeed;
    private float floatWaviness;
    private float floatHeight;

    private float shakeSpeed;
    private float shakeMaxOffset;

    List<startAndEnd> StartAndEnds = new List<startAndEnd>();
    public List<startAndEnd> startAndEnds => StartAndEnds;

    private VertexAnim[] vertexAnim;
    private int shakeFrameAmount = 128;

    public enum EffectType { Float, Shake }

    public struct startAndEnd
    {
        public int start;
        public int end;
        public EffectType type;
    }

    private struct VertexAnim
    {
        public float xOffset;
        public float yOffset;
    }

    private void Start()
    {
        vertexAnim = new VertexAnim[shakeFrameAmount];
        for (int i = 0; i < shakeFrameAmount; i++)
        {
            vertexAnim[i].xOffset = Random.Range(-1f, 1f);
            vertexAnim[i].yOffset = Random.Range(-1f, 1f);
        }

        floatSpeed = TextEffectManager.instance.floatSpeed;
        floatWaviness = TextEffectManager.instance.floatWaviness;
        floatHeight = TextEffectManager.instance.floatHeight;

        shakeSpeed = TextEffectManager.instance.shakeSpeed;
        shakeMaxOffset = TextEffectManager.instance.shakeMaxOffset;
    }

    public void ClearStartAndEnd()
    {
        startAndEnds.Clear();
    }

    public void AddStartAndEnd(startAndEnd startAndEnd)
    {
        startAndEnds.Add(startAndEnd);
    }

    //public IEnumerator EffectCoroutine()
    //{
    //    while (true)
    //    {
    //        UpdateEffect();

    //        yield return null;
    //    }
    //}

    private void LateUpdate()
    {
        if (startAndEnds.Count != 0)
            UpdateEffect();
    }

    public void UpdateEffect()
    {
        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;

        foreach (var item in startAndEnds)
        {
            int tempEnd = item.end;
            if (tempEnd > textInfo.characterCount)
            {
                tempEnd = textInfo.characterCount;
            }

            for (int i = item.start; i < tempEnd; i++)
            {
                var charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                    continue;

                var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

                for (int j = 0; j < 4; j++)
                {
                    var orig = verts[charInfo.vertexIndex + j];
                    verts[charInfo.vertexIndex + j] = orig + Offset(orig, i, item.type);
                }
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    protected virtual Vector3 Offset(Vector3 orig, int character, EffectType type)
    {
        switch (type)
        {
            default:
                return Vector3.zero;
            case EffectType.Float:
                return FloatOffset(orig);
            case EffectType.Shake:
                return ShakeOffset(character);
        }
    }

    private Vector3 FloatOffset(Vector3 orig)
    {
        return new Vector3(0, Mathf.Sin(Time.time * floatSpeed + orig.x * floatWaviness) * floatHeight);
    }

    private Vector3 ShakeOffset(int character)
    {
        int index = ((int)(Time.time * shakeSpeed) + character) % shakeFrameAmount;
        VertexAnim values = vertexAnim[index];
        VertexAnim previousValues;
        if (index == 0)
            previousValues = vertexAnim[shakeFrameAmount - 1];
        else
            previousValues = vertexAnim[index - 1];

        return new Vector3(SmoothStep(previousValues.xOffset * shakeMaxOffset, values.xOffset * shakeMaxOffset, shakeSpeed),
            SmoothStep(previousValues.yOffset * shakeMaxOffset, values.yOffset * shakeMaxOffset, shakeSpeed));
    }

    private float SmoothStep(float start, float end, float speed)
    {
        return Mathf.SmoothStep(start, end, (Time.time * speed) % 1);
    }
}
