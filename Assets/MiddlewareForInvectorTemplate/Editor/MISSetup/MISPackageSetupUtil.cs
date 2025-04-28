#if INVECTOR_BASIC
using Invector;
using Invector.vEventSystems;
using static Invector.vEventSystems.vAnimatorTagAdvanced;
#endif
#if INVECTOR_MELEE || INVECTOR_SHOOTER
using Invector.vItemManager;
#endif
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using System.IO;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public static class MISSetupUtil
    {
#if INVECTOR_BASIC
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static vBodySnappingControl GetBodySnappingControl(this Transform parent)
        {
            vBodySnappingControl bodySnappingControl = parent.GetComponentInChildren<vBodySnappingControl>();
            if (bodySnappingControl == null)
            {
                GameObject bodySnappingObject = new GameObject("BodySnaps");
                bodySnappingObject.transform.parent = parent;
                bodySnappingObject.transform.localPosition = Vector3.zero;
                bodySnappingObject.transform.localRotation = Quaternion.identity;
                bodySnappingControl = bodySnappingObject.AddComponent<vBodySnappingControl>();
            }

            return bodySnappingControl;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static vSnapToBody CreateSnapToBody(this vBodySnappingControl bodySnappingControl, string boneName)
        {
            GameObject snapToBodyObject = new GameObject(boneName);
            snapToBodyObject.transform.parent = bodySnappingControl.transform;
            snapToBodyObject.transform.localPosition = Vector3.zero;
            snapToBodyObject.transform.localRotation = Quaternion.identity;

            vSnapToBody snapToBody = snapToBodyObject.AddComponent<vSnapToBody>();
            snapToBody.bodySnap = bodySnappingControl;
            snapToBody.boneName = boneName;
            snapToBody.boneToSnap = bodySnappingControl.GetBone(boneName);

            return snapToBody;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static vSnapToBody GetSnapToBody(this vBodySnappingControl bodySnappingControl, string boneName)
        {
            vSnapToBody[] snapToBodys = bodySnappingControl.transform.GetComponentsInChildren<vSnapToBody>();
            vSnapToBody snapToBody = null;

            if (snapToBodys == null || snapToBodys.Length == 0)
            {
                snapToBody = bodySnappingControl.CreateSnapToBody(boneName);
                return snapToBody;
            }

            for (int i = 0; i < snapToBodys.Length; i++)
            {
                if (snapToBodys[i].boneName.Equals(boneName))
                    return snapToBodys[i];
            }

            snapToBody = bodySnappingControl.CreateSnapToBody(boneName);
            return snapToBody;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static Transform CreateSnapToBodyHandlerIfNotExist(this vSnapToBody snapToBody, string handlerName)
        {
            if (string.IsNullOrEmpty(handlerName))
                return null;

            GameObject handlerObj = null;
            Transform handlerTransform = snapToBody.gameObject.transform.Find(handlerName);

            if (handlerTransform == null)
            {
                handlerObj = new GameObject(handlerName);
                handlerObj.transform.SetParent(snapToBody.gameObject.transform);
                handlerObj.transform.localPosition = Vector3.zero;
                handlerObj.transform.localRotation = Quaternion.identity;
            }
            else
            {
                handlerObj = handlerTransform.gameObject;
            }

            return handlerObj.transform;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void RemoveAnimatorTagAdvancedIfExist(this vAnimatorTagAdvanced animatorTagAdvanced, string tagName)
        {
            vAdvancedTags target = animatorTagAdvanced.tags.Find(x => x.tagName.Equals(tagName));

            if (target != null)
                animatorTagAdvanced.tags.Remove(target);
        }
        public static vAdvancedTags AddAnimatorTagAdvancedIfNotExist(this vAnimatorTagAdvanced animatorTagAdvanced,
            string tagName, vAnimatorEventTriggerType tagType, Vector2 normalizedTime)
        {
            if (animatorTagAdvanced.tags == null)
                animatorTagAdvanced.tags = new List<vAdvancedTags>();

            if (animatorTagAdvanced.tags.Find(x => x.tagName.Equals(tagName) && x.tagType == tagType) == null)
            {
                var newAdvancedTag = new vAdvancedTags(tagName);
                newAdvancedTag.tagType = tagType;

                if (tagType == vAnimatorEventTriggerType.AllByNormalizedTime)
                    newAdvancedTag.normalizedTime = normalizedTime;

                animatorTagAdvanced.tags.Add(newAdvancedTag);

                return newAdvancedTag;
            }

            return null;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void AddvTriggerSoundByState(this AnimatorStateMachine stateMachine, List<AudioClip> audioClipList)
        {
            if (!stateMachine.TryGetStateMachineBehaviour(out vTriggerSoundByState triggerSoundByState))
                triggerSoundByState = stateMachine.AddStateMachineBehaviour<vTriggerSoundByState>();

            if (triggerSoundByState.audioSource == null)
                triggerSoundByState.audioSource = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(MISEditor.INVECTOR_ASSETS_PATH, "Basic Locomotion/Scripts/FootStep/AudioSource/AudioSource.prefab"));

            if (triggerSoundByState.sounds == null || (triggerSoundByState.sounds != null && triggerSoundByState.sounds.Count == 0))
                triggerSoundByState.sounds = audioClipList;

            triggerSoundByState.triggerTime = 0f;
        }
        public static void AddvTriggerSoundByState(this AnimatorState state, List<AudioClip> audioClipList)
        {
            if (!state.TryGetStateMachineBehaviour(out vTriggerSoundByState triggerSoundByState))
                triggerSoundByState = state.AddStateMachineBehaviour<vTriggerSoundByState>();
            
            if (triggerSoundByState.audioSource == null)
                triggerSoundByState.audioSource = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(MISEditor.INVECTOR_ASSETS_PATH, "Basic Locomotion/Scripts/FootStep/AudioSource/AudioSource.prefab"));
            
            if (triggerSoundByState.sounds == null || (triggerSoundByState.sounds != null && triggerSoundByState.sounds.Count == 0))
                triggerSoundByState.sounds = audioClipList;

            triggerSoundByState.triggerTime = 0f;
        }
#endif

#if INVECTOR_MELEE || INVECTOR_SHOOTER
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static int GetUniqueID(this vItemListData itemList, int value = 0)
        {
            return GetUniqueID(itemList.items);
        }
        public static int GetUniqueID(List<vItem> items, int value = 0)
        {
            var result = value;

            for (int i = 0; i < items.Count + 1; i++)
            {
                var item = items.Find(t => t.id == i);
                if (item == null)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void OrderByID(ref List<vItem> items)
        {
            if (items != null && items.Count > 0)
                items = items.OrderBy(i => i.id).ToList();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void AddvItem(this vItemListData itemList, vItem item)
        {
            if (item.name.Contains("(Clone)"))
                item.name = item.name.Replace("(Clone)", string.Empty);

            if (item && !itemList.items.Find(it => it.name.ToClearUpper().Equals(item.name.ToClearUpper())))
            {
                AssetDatabase.AddObjectToAsset(item, AssetDatabase.GetAssetPath(itemList));
                item.hideFlags = HideFlags.HideInHierarchy;

                if (itemList.items.Exists(it => it.id.Equals(item.id)))
                    item.id = itemList.GetUniqueID();
                itemList.items.Add(item);
                OrderByID(ref itemList.items);

                EditorUtility.SetDirty(itemList);
                AssetDatabase.SaveAssets();
            }
        }
#endif
    }

    /*
    public static Object DuplicateGameObject(this GameObject sourceObject)
    {
        Object prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(sourceObject);

        if (prefabRoot != null)
            return PrefabUtility.InstantiatePrefab(prefabRoot);
        else
            return Instantiate(sourceObject);
    }*/
}
