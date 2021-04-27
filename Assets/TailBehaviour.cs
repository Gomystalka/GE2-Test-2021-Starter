using UnityEngine;

//そのスクリプトは長くないでしょ
public class TailBehaviour : MonoBehaviour
{
    public float wagRate;
    public float wagAmplitude;

    void Update() => transform.localEulerAngles = transform.localEulerAngles.ReplaceY(Mathf.Sin(Time.time * wagRate) * wagAmplitude);
}

public static class ExtensionsUwU {
    public static Vector3 ReplaceY(this Vector3 v, float newY) => new Vector3(v.x, newY, v.z); 
}