
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;

using SoxwareInteractive.AnimationConversion;
using UniGLTF;
using UnityEditor.Animations;
using UnityEngine.Windows;
using UnityEngine.XR;
using Debug = UnityEngine.Debug;
using Directory = System.IO.Directory;
using Object = UnityEngine.Object;



public class BruEditor : Editor
{
    
    public Bru myScript;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        myScript = (Bru)target;
        myScript.ikd();
        if(GUILayout.Button("Build Object"))
        {
            myScript.BuildObject();
        }
    }

    public void bruvv()
    {
        
    }
}


public class Widget : Editor
{
    public static List<SkinnedMeshRenderer> skinnedList = new List<SkinnedMeshRenderer>();
    public static List<MeshFilter> FilterList = new List<MeshFilter>();
    public static List<Mesh> sharedList = new List<Mesh>();
    public static List<string> stringList = new List<string>();
    private static AnimationConverter.PrefabPair pair;
    static string path2 = "Assets/Safe";
    private static GameObject newPrefab;
    public static AnimationConverter.PrefabPair GetObject(GameObject toPrefab)
    {
        
        
        bool continu = false;

        foreach (Transform child in toPrefab.transform)
        {
            if (child.TryGetComponent(out SkinnedMeshRenderer skinned) && !stringList.Contains(path2))
            {
                
                path2 = AssetDatabase.GetAssetPath(skinned.sharedMesh);
                stringList.Add(path2);
                continu = true;
                break;
            }

            if(child.TryGetComponent(out MeshFilter renderer) && !stringList.Contains(path2))
            {
            
                path2 = AssetDatabase.GetAssetPath(renderer.sharedMesh);
                stringList.Add(path2);
                continu = true;

                break;
            }
        }

        if (continu)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path2, typeof(GameObject)) as GameObject;
            AssetDatabase.CopyAsset(path2, "Assets/Unity to Three.js/Temporary/Mesh/" + toPrefab.name + ".fbx");
            newPrefab = AssetDatabase.LoadAssetAtPath("Assets/Unity to Three.js/Temporary/Mesh/" + toPrefab.name + ".fbx", typeof(GameObject)) as GameObject;

            AssetDatabase.Refresh();

           // var original = AssetDatabase.LoadAssetAtPath(path2, prefab.GetType()) as GameObject;

           
            pair.SourcePrefab = prefab;
            pair.DestinationPrefab = newPrefab;

           
            
        }


        Replace(toPrefab, newPrefab);
        
        return pair;
        
    }


   // [MenuItem("Unity To Three/Convert Animation")] 
    private static void NewMenuOption()
    {
        
        Debug.Log("Conversion");
        
        var go = Selection.activeObject as GameObject;
        

        if (go != null)
        {
            

            foreach (Transform child in go.transform)
            {
                if (child.childCount > 0)
                {
                    foreach (Transform child2 in child.transform)
                    {
                        if (child2.TryGetComponent(out Animator a))
                        {
                            Debug.Log(child2.name);
                            Convert(child2);
                        }
                    }
                }

                if (child.TryGetComponent(out Animator ab))
                {
                    Debug.Log(child.name);
                    Convert(child);
                }
            }
            
            if (go.TryGetComponent(out Animator parentAnimator))
            {
                Convert(go.transform);
            }
        }
    }

    private static List<GameObject> newObjects = new List<GameObject>();
    private static void Replace(GameObject toReplace,GameObject replacement)
    {
        //replace object
        var selected = toReplace;
        var prefabType = PrefabUtility.GetPrefabAssetType(replacement);
        GameObject newObject;

        if (prefabType == PrefabAssetType.Regular)
        {
            newObject = (GameObject)PrefabUtility.InstantiatePrefab(replacement);
        }
        else
        {
            newObject = Instantiate(replacement);
            newObject.name = replacement.name;
        }

        if (newObject == null)
        {
            Debug.LogError("Error instantiating prefab");
          
        }

        Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
        newObject.transform.parent = selected.transform.parent;
        newObject.transform.localPosition = selected.transform.localPosition;
        newObject.transform.localRotation = selected.transform.localRotation;
        newObject.transform.localScale = selected.transform.localScale;
        newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
        
        newObjects.Add(newObject);
   
        Undo.DestroyObjectImmediate(selected);
    }
    
    
    public static void Convert(Transform toConvert)
    {
        
            AnimationClip[] l;
            Debug.Log(toConvert);
            l = AnimationUtility.GetAnimationClips(toConvert.gameObject);
            var pref = GetObject(toConvert.gameObject);
            
            for (int i = 0; i < l.Length; i++)
            {

                AnimationConverter.Configuration config = new AnimationConverter.Configuration();

                config.Prefabs = new AnimationConverter.PrefabPair[] {pref};
                config.DestinationAnimationType = AnimationConverter.AnimationType.Generic;
                config.OutputDirectory = "Assets/Unity to Three.js/Temporary/Animations";
                if (AnimationConverter.GetAnimationType(l[i]) == AnimationConverter.AnimationType.Humanoid)
                {
                    AnimationConverter.Convert(l[i], config, out string b);
                }
            }

            ApplyAnimator();
        
    }

    private static void ApplyAnimator()
    {
        AnimationClip[] animationClips;
        foreach (var newObject in newObjects)
        {

            Debug.Log(newObject.name);
            if (newObject.TryGetComponent(out Animator animator))
            {
                var controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Unity to Three.js/Temporary/Animations/Controllers/Controller.controller");

                var rootStateMachine = controller.layers[0].stateMachine;
                
                var files = Directory.GetFiles("Assets/Unity to Three.js/Temporary/Animations");

                
                foreach (var file in files)
                {
                    if (!file.EndsWith(".meta"))
                    {
                        var asset = AssetDatabase.LoadAssetAtPath(file,typeof(AnimationClip)) as AnimationClip;
                        var stateA1 = rootStateMachine.AddState("stateA1");
                        stateA1.motion = asset;
                        Debug.Log(asset.name);
                    }

                   // controller.animationClips = animationClips;

                }

                animator.runtimeAnimatorController = controller;

            }
        }
    }
   // [MenuItem("Unity To Three/Reimport")]
    private static void Reimport()
    {
        AssetDatabase.ImportAsset("Assets/Unity to Three.js/Temporary/Stickman_sphere_Criminal_01.fbx");
        
    }
}






