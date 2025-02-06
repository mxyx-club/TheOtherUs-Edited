using TMPro;
using UnityEngine;

namespace TheOtherRoles.Helper;

public static class ObjectHelper
{
    /// <summary>
    /// オブジェクトの<see cref="TextTranslatorTMP"/>コンポーネントを破棄します
    /// </summary>
    public static void DestroyTranslator(this GameObject obj)
    {
        if (obj == null) return;
        obj.ForEachChild((Il2CppSystem.Action<GameObject>)DestroyTranslator);
        TextTranslatorTMP[] translator = obj.GetComponentsInChildren<TextTranslatorTMP>(true);
        translator?.Do(Object.Destroy);
    }
    /// <summary>
    /// オブジェクトの<see cref="TextTranslatorTMP"/>コンポーネントを破棄します
    /// </summary>
    public static void DestroyTranslatorL(this MonoBehaviour obj) => obj?.gameObject?.DestroyTranslator();

    public static void SetActive(this Transform tf, bool b) => tf.gameObject.SetActive(b);
    public static void SetActive(this TextMeshPro tf, bool b) => tf.gameObject.SetActive(b);
}