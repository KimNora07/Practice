using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class EditorOnlySee : MonoBehaviour
{
    void OnEnable()
    {
        // Set the tag to "EditorOnly"
        gameObject.tag = "EditorOnly";
    }
}
