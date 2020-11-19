using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoxwareInteractive.AnimationConversion;
using UniGLTF;
using UnityEditor;

public class Bru : MonoBehaviour 
{
    public GameObject obj;
    public Vector3 spawnPoint;


    public int ikd()
    {
      //  AnimationConverter.Configuration tes = new AnimationConverter.Configuration();
       // tes.DestinationAnimationType = AnimationConverter.AnimationType.Generic;
      //  tes.Prefabs = new AnimationConverter.PrefabPair[];
            
            
        // foreach (var clip in UniGLTF.AnimationExporter.GetAnimationClips())
        // {
        //     Debug.Log(clip.name);
        //     AnimationConverter.Convert(clip,tes,out string k );
        // }

        return 4;
        //AnimationConverter.Convert(clip);
    }
    public void BuildObject()
    {
        Debug.Log("bbbb");
        var go = Selection.activeObject as GameObject;

        if (go != null)
        {
            foreach (Transform child in go.transform)
            {
                if (child.TryGetComponent(out Animator animator))
                {
                    Debug.Log(animator);
                }

                else if (child.TryGetComponent(out Animation animation))

                {
                    Debug.Log(animation);
                }
            }
        }
    }
}
