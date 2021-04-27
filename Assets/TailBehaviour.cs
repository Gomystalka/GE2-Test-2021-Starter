using UnityEngine;

//そのスクリプトは長くないでしょ
/// <summary>
/// Tail Behaviour script by Tomasz Galka C18740411
/// </summary>
public class TailBehaviour : MonoBehaviour
{
    public float wagRate;
    public float wagAmplitude;
    public const float kTwoPi = Mathf.PI * 2f;

    private float _internalRate;

    void Update()
    {
        transform.localEulerAngles = transform.localEulerAngles.ReplaceY(Mathf.Sin(_internalRate) * wagAmplitude);
        _internalRate += kTwoPi * Time.deltaTime * wagRate * 0.5f;
    }
}

public static class ExtensionsUwU {
    public static Vector3 ReplaceY(this Vector3 v, float newY) => new Vector3(v.x, newY, v.z); 
}