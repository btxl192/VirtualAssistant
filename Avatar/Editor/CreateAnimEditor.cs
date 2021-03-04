using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LipSync))]
public class CreateAnimEditor : Editor
{
    private const string blendshapePrefix = "blendShape.";
    private const int pixelSpace = 20;

    private string[] template = new string[]
    {
            "%YAML 1.1",
            "%TAG !u! tag:unity3d.com,2011:",
            "--- !u!74 &7400000",
            "AnimationClip:",
            "  m_ObjectHideFlags: 0",
            "  m_CorrespondingSourceObject: {fileID: 0}",
            "  m_PrefabInstance: {fileID: 0}",
            "  m_PrefabAsset: {fileID: 0}",
            "  m_Name: anim_name_here",
            "  serializedVersion: 6",
            "  m_Legacy: 0",
            "  m_Compressed: 0",
            "  m_UseHighQualityCurve: 1",
            "  m_RotationCurves: []",
            "  m_CompressedRotationCurves: []",
            "  m_EulerCurves: []",
            "  m_PositionCurves: []",
            "  m_ScaleCurves: []",
            "  m_FloatCurves:",
            "  - curve:",
            "      serializedVersion: 2",
            "      m_Curve:",
            "      - serializedVersion: 3",
            "        time: 0",
            "        value: insertvalue_a",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      - serializedVersion: 3",
            "        time: 1",
            "        value: 0",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      m_PreInfinity: 2",
            "      m_PostInfinity: 2",
            "      m_RotationOrder: 4",
            "    attribute: attribute_name_a",
            "    path: mouth_path",
            "    classID: 137",
            "    script: {fileID: 0}",
            "  - curve:",
            "      serializedVersion: 2",
            "      m_Curve:",
            "      - serializedVersion: 3",
            "        time: 0",
            "        value: insertvalue_e",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      - serializedVersion: 3",
            "        time: 1",
            "        value: 0",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      m_PreInfinity: 2",
            "      m_PostInfinity: 2",
            "      m_RotationOrder: 4",
            "    attribute: attribute_name_e",
            "    path: mouth_path",
            "    classID: 137",
            "    script: {fileID: 0}",
            "  - curve:",
            "      serializedVersion: 2",
            "      m_Curve:",
            "      - serializedVersion: 3",
            "        time: 0",
            "        value: insertvalue_i",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      - serializedVersion: 3",
            "        time: 1",
            "        value: 0",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      m_PreInfinity: 2",
            "      m_PostInfinity: 2",
            "      m_RotationOrder: 4",
            "    attribute: attribute_name_i",
            "    path: mouth_path",
            "    classID: 137",
            "    script: {fileID: 0}",
            "  - curve:",
            "      serializedVersion: 2",
            "      m_Curve:",
            "      - serializedVersion: 3",
            "        time: 0",
            "        value: insertvalue_o",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      - serializedVersion: 3",
            "        time: 1",
            "        value: 0",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      m_PreInfinity: 2",
            "      m_PostInfinity: 2",
            "      m_RotationOrder: 4",
            "    attribute: attribute_name_o",
            "    path: mouth_path",
            "    classID: 137",
            "    script: {fileID: 0}",
            "  - curve:",
            "      serializedVersion: 2",
            "      m_Curve:",
            "      - serializedVersion: 3",
            "        time: 0",
            "        value: insertvalue_u",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      - serializedVersion: 3",
            "        time: 1",
            "        value: 0",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      m_PreInfinity: 2",
            "      m_PostInfinity: 2",
            "      m_RotationOrder: 4",
            "    attribute: attribute_name_u",
            "    path: mouth_path",
            "    classID: 137",
            "    script: {fileID: 0}",
            "  m_PPtrCurves: []",
            "  m_SampleRate: 60",
            "  m_WrapMode: 0",
            "  m_Bounds:",
            "    m_Center: {x: 0, y: 0, z: 0}",
            "    m_Extent: {x: 0, y: 0, z: 0}",
            "  m_ClipBindingConstant:",
            "    genericBindings:",
            "    - serializedVersion: 2",
            "      path: 1850410951",
            "      attribute: 1783637695",
            "      script: {fileID: 0}",
            "      typeID: 137",
            "      customType: 20",
            "      isPPtrCurve: 0",
            "    - serializedVersion: 2",
            "      path: 1850410951",
            "      attribute: 1832772262",
            "      script: {fileID: 0}",
            "      typeID: 137",
            "      customType: 20",
            "      isPPtrCurve: 0",
            "    pptrCurveMapping: []",
            "  m_AnimationClipSettings:",
            "    serializedVersion: 2",
            "    m_AdditiveReferencePoseClip: {fileID: 0}",
            "    m_AdditiveReferencePoseTime: 0",
            "    m_StartTime: 0",
            "    m_StopTime: 1",
            "    m_OrientationOffsetY: 0",
            "    m_Level: 0",
            "    m_CycleOffset: 0",
            "    m_HasAdditiveReferencePose: 0",
            "    m_LoopTime: 1",
            "    m_LoopBlend: 0",
            "    m_LoopBlendOrientation: 0",
            "    m_LoopBlendPositionY: 0",
            "    m_LoopBlendPositionXZ: 0",
            "    m_KeepOriginalOrientation: 0",
            "    m_KeepOriginalPositionY: 1",
            "    m_KeepOriginalPositionXZ: 0",
            "    m_HeightFromFeet: 0",
            "    m_Mirror: 0",
            "  m_EditorCurves:",
            "  - curve:",
            "      serializedVersion: 2",
            "      m_Curve:",
            "      - serializedVersion: 3",
            "        time: 0",
            "        value: insertvalue_a",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      - serializedVersion: 3",
            "        time: 1",
            "        value: 0",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      m_PreInfinity: 2",
            "      m_PostInfinity: 2",
            "      m_RotationOrder: 4",
            "    attribute: attribute_name_a",
            "    path: mouth_path",
            "    classID: 137",
            "    script: {fileID: 0}",
            "  - curve:",
            "      serializedVersion: 2",
            "      m_Curve:",
            "      - serializedVersion: 3",
            "        time: 0",
            "        value: insertvalue_e",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      - serializedVersion: 3",
            "        time: 1",
            "        value: 0",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      m_PreInfinity: 2",
            "      m_PostInfinity: 2",
            "      m_RotationOrder: 4",
            "    attribute: attribute_name_e",
            "    path: mouth_path",
            "    classID: 137",
            "    script: {fileID: 0}",
            "  - curve:",
            "      serializedVersion: 2",
            "      m_Curve:",
            "      - serializedVersion: 3",
            "        time: 0",
            "        value: insertvalue_i",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      - serializedVersion: 3",
            "        time: 1",
            "        value: 0",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      m_PreInfinity: 2",
            "      m_PostInfinity: 2",
            "      m_RotationOrder: 4",
            "    attribute: attribute_name_i",
            "    path: mouth_path",
            "    classID: 137",
            "    script: {fileID: 0}",
            "  - curve:",
            "      serializedVersion: 2",
            "      m_Curve:",
            "      - serializedVersion: 3",
            "        time: 0",
            "        value: insertvalue_o",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      - serializedVersion: 3",
            "        time: 1",
            "        value: 0",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      m_PreInfinity: 2",
            "      m_PostInfinity: 2",
            "      m_RotationOrder: 4",
            "    attribute: attribute_name_o",
            "    path: mouth_path",
            "    classID: 137",
            "    script: {fileID: 0}",
            "  - curve:",
            "      serializedVersion: 2",
            "      m_Curve:",
            "      - serializedVersion: 3",
            "        time: 0",
            "        value: insertvalue_u",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      - serializedVersion: 3",
            "        time: 1",
            "        value: 0",
            "        inSlope: 0",
            "        outSlope: 0",
            "        tangentMode: 136",
            "        weightedMode: 0",
            "        inWeight: 0.33333334",
            "        outWeight: 0.33333334",
            "      m_PreInfinity: 2",
            "      m_PostInfinity: 2",
            "      m_RotationOrder: 4",
            "    attribute: attribute_name_u",
            "    path: mouth_path",
            "    classID: 137",
            "    script: {fileID: 0}",
            "  m_EulerEditorCurves: []",
            "  m_HasGenericRootTransform: 0",
            "  m_HasMotionFloatCurves: 0",
            "  m_Events: []"
       };
    private bool toShow = false;

    [SerializeField]
    private Transform mouth;    
    [SerializeField]
    private string aBlendshapeName = "";
    [SerializeField]
    private string eBlendshapeName = "";
    [SerializeField]
    private string iBlendshapeName = "";
    [SerializeField]
    private string oBlendshapeName = "";
    [SerializeField]
    private string uBlendshapeName = "";

    private string currentSymbol = "";

    enum Vowels
    {
        a = 0,
        e = 1,
        i = 2,
        o = 3,
        u = 4
    }

    Dictionary<string, int[]> defaultShapeMappings = new Dictionary<string, int[]>()
    {
        { "A", new int[] {100,0,0,0,0} },
        { "d", new int[] {0, 0, 75, 0, 0 } },
        { "E", new int[] {20,75,0,0,0 } },
        { "f", new int[] {0,75,75,0,0} },
        { "g", new int[] {0,75,0,0,0} },
        { "I", new int[] {50,0,100,0,0} },
        { "j", new int[] {0,0,0,0,75} },
        { "m", new int[] {0,100,0,0,0} },
        { "n", new int[] {0,100,0,0,0} },
        { "none", new int[] {0,0,100,0,0} },
        { "O", new int[] {0,0,0,100,0} },
        { "p", new int[] {0,0,100,0,0} },
        { "s", new int[] {0,0,100,0,0} },
        { "default", new int[] {0,0,0,0,0} },
        { "t", new int[] {0,0,75,0,0} },
        { "U", new int[] {0,0,0,0,100} },
        { "w", new int[] {0,0,0,0,75} },
        { "æ", new int[] {100,0,0,0,0} },
        { "ð", new int[] {0,0,75,0,0} },
        { "ə", new int[] {20,75,0,0,0} },
        { "ɛ", new int[] {10,75,0,0,0} },
        { "ɪ", new int[] {20,0,100,0,0} },
        { "ʊ", new int[] {0,0,0,0,100} },
        { "θ", new int[] {0,75,75,0,0} }
    };

    string[] vars = new string[] 
    {
        "attribute_name_a",
        "attribute_name_e",
        "attribute_name_i",
        "attribute_name_o",
        "attribute_name_u",
        "insertvalue_a",
        "insertvalue_e",
        "insertvalue_i",
        "insertvalue_o",
        "insertvalue_u",
        "anim_name_here",
        "mouth_path"
    };

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        toShow = EditorGUILayout.Foldout(toShow, "Animation Generation");

        if (toShow)
        {
            mouth = (Transform)EditorGUILayout.ObjectField("Mouth object: ", mouth, typeof(Transform), true);
            aBlendshapeName = EditorGUILayout.TextField("BlendShape name A: ", aBlendshapeName).Trim();
            eBlendshapeName = EditorGUILayout.TextField("BlendShape name E: ", eBlendshapeName).Trim();
            iBlendshapeName = EditorGUILayout.TextField("BlendShape name I: ", iBlendshapeName).Trim();
            oBlendshapeName = EditorGUILayout.TextField("BlendShape name O: ", oBlendshapeName).Trim();
            uBlendshapeName = EditorGUILayout.TextField("BlendShape name U: ", uBlendshapeName).Trim();

            if (mouth != null && aBlendshapeName != "" && eBlendshapeName != "" && iBlendshapeName != "" && oBlendshapeName != "" && uBlendshapeName != "")
            {
                string outputfolder = Path.Combine(Directory.GetCurrentDirectory(), "Assets", mouth.root.name + "_mouthanims");
                GUILayout.Space(pixelSpace);
                if (GUILayout.Button("Generate Animations"))
                {
                    foreach (string symbol in defaultShapeMappings.Keys)
                    {
                        currentSymbol = symbol;
                        List<string> lines = new List<string>();

                        foreach (string line in template)
                        {
                            string toWrite = line;
                            
                            foreach (string v in vars)
                            {
                                if (line.Contains(v))
                                {
                                    toWrite = replaceVariable(v, line);
                                }
                            }
                            
                            lines.Add(toWrite);
                        }
                        if (!Directory.Exists(outputfolder))
                        {
                            Directory.CreateDirectory(outputfolder);
                        }
                        System.IO.File.WriteAllLines(Path.Combine(outputfolder, GetAnimName() + ".anim"), lines);
                    }

                    Debug.Log("Done creating animations! Output to " + outputfolder + ". Please refresh the Unity file explorer");
                }
            }
            else
            {
                GUILayout.Space(pixelSpace);
                GUILayout.Label("Fields incomplete! Cannot generate animations");
            }
            
        }
        if (!Selection.activeTransform)
        {
            toShow = false;
        }

        

    }

    public void OnInspectorUpdate()
    {
        this.Repaint();
    }

    private string GetMouthPath()
    {
        string path = mouth.name;
        Transform t = mouth;
        while (t.parent != null)
        {
            t = t.parent;
            if (t != ((LipSync)target).transform)
            {
                path = t.name + "/" + path;
            }
            else
            {
                break;
            }
        }
        return path;
    }

    private string GetAnimName()
    {
        return "MTH_" + currentSymbol;
    }

    private string GetShapeMapping(Vowels v)
    {
        return defaultShapeMappings[currentSymbol][(int)v].ToString();
    }

    private string replaceVariable(string v, string line)
    {
        switch (v)
        {
            case "attribute_name_a":
                return line.Replace(v, blendshapePrefix + aBlendshapeName);
            case "attribute_name_e":
                return line.Replace(v, blendshapePrefix + eBlendshapeName);
            case "attribute_name_i":
                return line.Replace(v, blendshapePrefix + iBlendshapeName);
            case "attribute_name_o":
                return line.Replace(v, blendshapePrefix + oBlendshapeName);
            case "attribute_name_u":
                return line.Replace(v, blendshapePrefix + uBlendshapeName);
            case "insertvalue_a":
                return line.Replace(v, GetShapeMapping(Vowels.a));
            case "insertvalue_e":
                return line.Replace(v, GetShapeMapping(Vowels.e));
            case "insertvalue_i":
                return line.Replace(v, GetShapeMapping(Vowels.i));
            case "insertvalue_o":
                return line.Replace(v, GetShapeMapping(Vowels.o));
            case "insertvalue_u":
                return line.Replace(v, GetShapeMapping(Vowels.u));
            case "anim_name_here":
                return line.Replace(v, GetAnimName());
            case "mouth_path":
                return line.Replace(v, GetMouthPath());
        }
        return null;
    }
}
