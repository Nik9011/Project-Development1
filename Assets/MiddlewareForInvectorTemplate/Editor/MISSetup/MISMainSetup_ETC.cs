using Invector.vCharacterController.vActions;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public partial class MISMainSetup
    {
#if INVECTOR_BASIC
        // ----------------------------------------------------------------------------------------------------
        // vLadder
        [SerializeField] protected List<vTriggerLadderAction> triggerLadderActionList;
        protected ReorderableList triggerLadderActionRL;
#endif

#if MIS_INVECTOR_PUSH
        // ----------------------------------------------------------------------------------------------------
        // vPushAction
        [SerializeField] protected List<vPushableObject> pushableObjectList;
        protected ReorderableList pushableObjectRL;

        [SerializeField] protected List<vPushObjectPoint> pushObjectPointList;
        protected ReorderableList pushObjectPointRL;
#endif

        protected ScriptableObject targetETC;
        protected SerializedObject soETC;
        protected Vector2 misETCScroll;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void OnEnableETC()
        {
            targetETC = this;
            soETC = new SerializedObject(targetETC);

#if INVECTOR_BASIC
            // ----------------------------------------------------------------------------------------------------
            // vTriggerLadderAction
            triggerLadderActionList = new List<vTriggerLadderAction>();
            triggerLadderActionRL = new ReorderableList(soETC, soETC.FindProperty("triggerLadderActionList"), false, false, false, false);

            /*
            triggerLadderActionRL.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "vTriggerLadderAction List", EditorStyles.boldLabel);
            };*/
            triggerLadderActionRL.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = triggerLadderActionRL.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };
            triggerLadderActionRL.drawNoneElementCallback = rect =>
            {
                EditorGUI.LabelField(rect, "No vTriggerLadderAction in Hierarchy");
            };
#endif


#if MIS_INVECTOR_PUSH
            // ----------------------------------------------------------------------------------------------------
            // vPushableObject
            pushableObjectList = new List<vPushableObject>();
            pushableObjectRL = new ReorderableList(soETC, soETC.FindProperty("pushableObjectList"), false, false, false, false);

            /*
            pushableObjectRL.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "vPushableObject List", EditorStyles.boldLabel);
            };*/
            pushableObjectRL.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = pushableObjectRL.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };
            pushableObjectRL.drawNoneElementCallback = rect =>
            {
                EditorGUI.LabelField(rect, "No vPushableObject in Hierarchy");
            };


            // ----------------------------------------------------------------------------------------------------
            // vPushObjectPoint
            pushObjectPointList = new List<vPushObjectPoint>();
            pushObjectPointRL = new ReorderableList(soETC, soETC.FindProperty("pushObjectPointList"), false, false, false, false);

            /*
            pushObjectPointRL.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "vPushObjectPoint List", EditorStyles.boldLabel);
            };*/
            pushObjectPointRL.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = pushObjectPointRL.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };
            pushObjectPointRL.drawNoneElementCallback = rect =>
            {
                EditorGUI.LabelField(rect, "No vPushObjectPoint in Hierarchy");
            };
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void ETCContent()
        {
            soETC.Update();

            GUILayout.BeginVertical(skin.GetStyle("WindowBG"));
            {
                EditorGUILayout.HelpBox("Setup other Invector components for MIS.", MessageType.Info);

                GUILayout.Space(5);

                // ----------------------------------------------------------------------------------------------------
                // Target
                GUILayout.BeginVertical("box");
                {
                    //GUILayout.Label("<b>Target</b>", GUILayout.MaxHeight(25));
                    CheckHumanoidOption();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.MaxHeight(35));
                        {
                            MISEditor.SetToggleLabelWidth(true, 200f);
                            characterObj = EditorGUILayout.ObjectField("(MIS)INVECTOR Character", characterObj, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
                            MISEditor.SetToggleLabelWidth(false);
                            hasMISController = HasMISController(true);

                            MISEditor.SetToggleLabelWidth(true, 200f);
                            cameraObj = EditorGUILayout.ObjectField("Main Camera", cameraObj, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
                            MISEditor.SetToggleLabelWidth(false);
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(10);

                        if (GUILayout.Button(iconView, GUILayout.Width(30), GUILayout.Height(25)))
                        {
                            if (RefreshInvectorCharacter() == false)
                                Debug.LogWarning("Please select a INVECTOR(MIS) character in Hierarchy first.");
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUILayout.Space(15);

                misETCScroll = EditorGUILayout.BeginScrollView(misETCScroll, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    LadderAction();

#if MIS_INVECTOR_PUSH
                    PushAction();
#endif
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndVertical();

            soETC.ApplyModifiedProperties();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void LadderAction()
        {
            GUILayout.BeginVertical("box");
            {
                // ----------------------------------------------------------------------------------------------------
                // vLadderAction
                GUILayout.Label("<b>vLadderAction</b>", GUILayout.MaxHeight(25));

                EditorGUI.BeginDisabledGroup(hasMISController == false);
                {
                    if (GUILayout.Button("Convert vLadderAction"))
                    {
#if UNITY_2018_3_OR_NEWER
                        PrefabAssetType m_AssetType = PrefabUtility.GetPrefabAssetType(characterObj);

                        if (m_AssetType != PrefabAssetType.NotAPrefab)
                        {
                            GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(characterObj);
                            PrefabUtility.UnpackPrefabInstance(prefabRoot, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                        }
#endif

                        ConvertLadderAction(characterObj);
                    }
                }
                EditorGUI.EndDisabledGroup();

                GUILayout.Space(10);

                GUILayout.BeginVertical("box");
                {
                    // ----------------------------------------------------------------------------------------------------
                    // vTriggerLadderAction
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("<b>vTriggerLadderAction</b>", GUILayout.MaxHeight(25));

                        GUILayout.FlexibleSpace();

                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button(iconView, GUILayout.Width(30), GUILayout.Height(25)))
                            {
                                FindTriggerLadderActions();
                            }

                            GUILayout.Space(5);

                            if (GUILayout.Button(iconRemove, GUILayout.Width(30), GUILayout.Height(25)))
                            {
                                ClearTriggerLadderActions();
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    EditorGUILayout.HelpBox("Click the Find icon to search all vTriggerLadderActions in Hierarchy.", MessageType.Info);

                    triggerLadderActionRL.DoLayoutList();

                    EditorGUI.BeginDisabledGroup(triggerLadderActionList == null || triggerLadderActionList.Count == 0);
                    {
                        if (GUILayout.Button("Convert vTriggerLadderAction"))
                        {
#if UNITY_2018_3_OR_NEWER
                            for (int i = 0; i < triggerLadderActionList.Count; i++)
                            {
                                PrefabAssetType m_AssetType = PrefabUtility.GetPrefabAssetType(triggerLadderActionList[i].gameObject);

                                if (m_AssetType != PrefabAssetType.NotAPrefab)
                                {
                                    GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(triggerLadderActionList[i]);
                                    PrefabUtility.UnpackPrefabInstance(prefabRoot, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                                }
                            }
#endif

                            ConvertTriggerLadderActions();
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ConvertLadderAction(GameObject characterObj)
        {
            if (characterObj.TryGetComponent(out vLadderAction sourceLadderAction) && sourceLadderAction.enabled && sourceLadderAction.IsTheType<vLadderAction>())
            {
                if (characterObj.TryGetComponent(out mvLadderAction targetLadderAction))
                    DestroyImmediate(targetLadderAction);
                targetLadderAction = characterObj.AddComponent<mvLadderAction>();

                MISEditor.PasteComponentValues(sourceLadderAction, targetLadderAction);

                if (sourceLadderAction)
                    DestroyImmediate(sourceLadderAction);

                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();

                MISSetupCompletePopup.Open();
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ConvertTriggerLadderActions()
        {
            for (int i = 0; i < triggerLadderActionList.Count; i++)
            {
                if (triggerLadderActionList[i].gameObject.TryGetComponent(out mvTriggerLadderAction targetPushableObject))
                    DestroyImmediate(targetPushableObject);
                targetPushableObject = triggerLadderActionList[i].gameObject.AddComponent<mvTriggerLadderAction>();

                MISEditor.PasteComponentValues(triggerLadderActionList[i], targetPushableObject);

                DestroyImmediate(triggerLadderActionList[i]);
            }

            triggerLadderActionList.Clear();

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            MISSetupCompletePopup.Open();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool FindTriggerLadderActions()
        {
            vTriggerLadderAction[] pushObjectPoints = GameObject.FindObjectsOfType<vTriggerLadderAction>();
            triggerLadderActionList = pushObjectPoints.ToList();

            for (int i = triggerLadderActionList.Count - 1; i >= 0; i--)
            {
                if (!triggerLadderActionList[i].IsTheType<vTriggerLadderAction>())
                    triggerLadderActionList.Remove(triggerLadderActionList[i]);
            }

            if (triggerLadderActionList == null || triggerLadderActionList.Count == 0)
                return false;

            return true;
        }
        protected virtual void ClearTriggerLadderActions()
        {
            triggerLadderActionList.Clear();
        }

#if MIS_INVECTOR_PUSH
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void PushAction()
        {
            GUILayout.Space(10);

            GUILayout.BeginVertical("box");
            {
                GUILayout.Label("<b>vPushAction</b>", GUILayout.MaxHeight(25));

                EditorGUILayout.HelpBox("Unlike other Invector add-ons, PushAction can be used without converting to MIS.\n" +
                    "This function only applies to users such as MIS-RowingBoat.", MessageType.Warning);

                GUILayout.Space(10);

                GUILayout.BeginVertical("box");
                {
                    // ----------------------------------------------------------------------------------------------------
                    // mvPushableObject
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("<b>vPushableObject</b>", GUILayout.MaxHeight(25));

                        GUILayout.FlexibleSpace();

                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button(iconView, GUILayout.Width(30), GUILayout.Height(25)))
                            {
                                FindPushableObjects();
                            }

                            GUILayout.Space(5);

                            if (GUILayout.Button(iconRemove, GUILayout.Width(30), GUILayout.Height(25)))
                            {
                                ClearPushObjectPoints();
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    EditorGUILayout.HelpBox("Click the Find icon to search all vPushableObjects in Hierarchy.", MessageType.Info);

                    pushableObjectRL.DoLayoutList();

                    EditorGUI.BeginDisabledGroup(pushableObjectList == null || pushableObjectList.Count == 0);
                    {
                        if (GUILayout.Button("Convert vPushableObject"))
                        {
#if UNITY_2018_3_OR_NEWER
                            for (int i = 0; i < pushableObjectList.Count; i++)
                            {
                                PrefabAssetType m_AssetType = PrefabUtility.GetPrefabAssetType(pushableObjectList[i].gameObject);

                                if (m_AssetType != PrefabAssetType.NotAPrefab)
                                {
                                    GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(pushableObjectList[i]);
                                    PrefabUtility.UnpackPrefabInstance(prefabRoot, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                                }
                            }
#endif

                            ConvertPushableObjects();
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
                GUILayout.EndVertical();

                GUILayout.Space(10);

                GUILayout.BeginVertical("box");
                {
                    // ----------------------------------------------------------------------------------------------------
                    // mvPushableObjectPoint
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("<b>vPushableObjectPoint</b>", GUILayout.MaxHeight(25));

                        GUILayout.FlexibleSpace();

                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button(iconView, GUILayout.Width(30), GUILayout.Height(25)))
                            {
                                FindPushObjectPoints();
                            }

                            GUILayout.Space(5);

                            if (GUILayout.Button(iconRemove, GUILayout.Width(30), GUILayout.Height(25)))
                            {
                                ClearPushObjectPoints();
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    EditorGUILayout.HelpBox("Click the Find icon to search all vPushableObjectPoints in Hierarchy.", MessageType.Info);

                    pushObjectPointRL.DoLayoutList();

                    EditorGUI.BeginDisabledGroup(pushObjectPointList == null || pushObjectPointList.Count == 0);
                    {
                        if (GUILayout.Button("Convert vPushableObjectPoint"))
                        {
#if UNITY_2018_3_OR_NEWER
                            for (int i = 0; i < pushObjectPointList.Count; i++)
                            {
                                PrefabAssetType m_AssetType = PrefabUtility.GetPrefabAssetType(pushObjectPointList[i].gameObject);

                                if (m_AssetType != PrefabAssetType.NotAPrefab)
                                {
                                    GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(pushObjectPointList[i]);
                                    PrefabUtility.UnpackPrefabInstance(prefabRoot, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                                }
                            }
#endif

                            ConvertPushableObjectPoints();
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool FindPushableObjects()
        {
            vPushableObject[] pushableObjects = GameObject.FindObjectsOfType<vPushableObject>();
            pushableObjectList = pushableObjects.ToList();

            for (int i = pushableObjectList.Count - 1; i >= 0; i--)
            {
                if (!pushableObjectList[i].IsTheType<vPushableObject>())
                    pushableObjectList.Remove(pushableObjectList[i]);
            }

            if (pushableObjectList == null || pushableObjectList.Count == 0)
                return false;

            return true;
        }
        protected virtual void ClearPushableObjects()
        {
            pushableObjectList.Clear();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ConvertPushableObjects()
        {
            for (int i = 0; i < pushableObjectList.Count; i++)
            {
                if (pushableObjectList[i].gameObject.TryGetComponent(out mvPushableObject targetPushableObject))
                    DestroyImmediate(targetPushableObject);
                targetPushableObject = pushableObjectList[i].gameObject.AddComponent<mvPushableObject>();

                MISEditor.PasteComponentValues(pushableObjectList[i], targetPushableObject);

                DestroyImmediate(pushableObjectList[i]);
            }

            pushableObjectList.Clear();

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            MISSetupCompletePopup.Open();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool FindPushObjectPoints()
        {
            vPushObjectPoint[] pushObjectPoints = GameObject.FindObjectsOfType<vPushObjectPoint>();
            pushObjectPointList = pushObjectPoints.ToList();

            for (int i = pushObjectPointList.Count - 1; i >= 0; i--)
            {
                if (!pushObjectPointList[i].IsTheType<vPushObjectPoint>())
                    pushObjectPointList.Remove(pushObjectPointList[i]);
            }

            if (pushObjectPointList == null || pushObjectPointList.Count == 0)
                return false;

            return true;
        }
        protected virtual void ClearPushObjectPoints()
        {
            pushObjectPointList.Clear();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ConvertPushableObjectPoints()
        {
            for (int i = 0; i < pushObjectPointList.Count; i++)
            {
                if (pushObjectPointList[i].gameObject.TryGetComponent(out mvPushObjectPoint targetPushObjectPoint))
                    DestroyImmediate(targetPushObjectPoint);
                targetPushObjectPoint = pushObjectPointList[i].gameObject.AddComponent<mvPushObjectPoint>();

                MISEditor.PasteComponentValues(pushObjectPointList[i], targetPushObjectPoint);

                DestroyImmediate(pushObjectPointList[i]);
            }

            pushObjectPointList.Clear();

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            MISSetupCompletePopup.Open();
        }
#endif
    }
}