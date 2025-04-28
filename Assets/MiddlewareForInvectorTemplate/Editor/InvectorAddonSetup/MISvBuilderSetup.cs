using Invector.vCharacterController;
using Invector.vEventSystems;
using static Invector.vEventSystems.vAnimatorEvent;
#if INVECTOR_SHOOTER
using Invector.vItemManager;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using static com.mobilin.games.MISAnimator;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public partial class MISMainSetup
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        bool useBuilderInventory;


        // ----------------------------------------------------------------------------------------------------
        // Animator StateMachine/State
        AnimatorStateMachine base_BuilderSM;
        AnimatorState base_PlantingMine;
        AnimatorState base_HammeringWood;
        AnimatorState base_Healing;
        AnimatorState base_HammeringTrap;
        AnimatorState base_BuildingCrouched;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void BuilderSetup(mvSetupOption setupOption, GameObject characterObj, GameObject cameraObj)
        {
#if INVECTOR_SHOOTER && MIS_INVECTOR_BUILDER
            // ----------------------------------------------------------------------------------------------------
            // Setup Options
            // ----------------------------------------------------------------------------------------------------
            useBuilderInventory = setupOption.HasSetupOption(SetupOption.BuilderInventory);


            // ----------------------------------------------------------------------------------------------------
            // Main Component
            // ----------------------------------------------------------------------------------------------------

            // ----------------------------------------------------------------------------------------------------
            // mvDrawHideShooterWeapons
            mvDrawHideShooterWeapons drawHideWeapons = null;
            if (templateType == MISEditor.TemplateType.Shooter)
            {
                drawHideWeapons = characterObj.GetComponent<mvDrawHideShooterWeapons>();
                if (drawHideWeapons == null)
                    drawHideWeapons = characterObj.AddComponent<mvDrawHideShooterWeapons>();
            }
            else
            {
                Debug.LogError("[INVECTOR Builder]This character is not a Shooter.");
                return;
            }


            // ----------------------------------------------------------------------------------------------------
            // BuildManager
            GameObject buildManagerObj;
            var buildManagerTransform = invectorComponentsParentObj.transform.Find("BuildManager");
            if (buildManagerTransform == null)
            {
                GameObject buildManagerPrefab = 
                    AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(MISFeature.MIS_INVECTOR_BUILDER_PATH, "Prefabs/BuildManager.prefab"));
                buildManagerObj = buildManagerPrefab.Instantiate3D(Vector3.zero, invectorComponentsParentObj.transform);
            }
            else
            {
                buildManagerObj = buildManagerTransform.gameObject;
            }

            // vBuildManager
            if (buildManagerObj.TryGetComponent(out vBuildManager buildManager) == false)
                buildManager = buildManagerObj.AddComponent<vBuildManager>();

            // onEnterBuildMode Event
            if (buildManager.onEnterBuildMode == null)
                buildManager.onEnterBuildMode = new UnityEvent();

            buildManager.onEnterBuildMode.RemoveMissingPersistents();

            if (buildManager.onEnterBuildMode.HasPersistent(drawHideWeapons, drawHideWeapons.GetType(), "ForceHideWeapons", typeof(bool)) == false)
            {
                UnityAction<bool> setActiveDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), drawHideWeapons, "ForceHideWeapons") as UnityAction<bool>;
                UnityEventTools.AddBoolPersistentListener(buildManager.onEnterBuildMode, setActiveDelegate, true);
            }


            // ----------------------------------------------------------------------------------------------------
            // Builder Inventory
            if (useBuilderInventory)
            {
                vInventory inventory = characterObj.GetComponentInChildren<vInventory>(true);

                // Inventory
                GameObject inventoryObj;
                if (inventory == null)
                {
                    GameObject inventoryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(MISEditor.INVECTOR_ASSETS_PATH, "ItemManager/Prefabs/Inventory_ShooterMelee.prefab"));
                    GameObject inventoryRootObj = inventoryPrefab.Instantiate3D(Vector3.zero, invectorComponentsParentObj.transform);
                    inventoryRootObj.name = "Inventory_ShooterMelee_Builder";
                    inventory = inventoryRootObj.GetComponentInChildren<vInventory>();
                }
                inventoryObj = inventory.gameObject;

                // EquipmentAreaWindow
                if (inventoryObj.GetComponentInChildren<vEquipArea>(true) == null)
                {
                    Debug.LogError("[INVECTOR Builder]EquipmentAreaWindow cannot be found. Please remove Inventory root object then run Setup again.");
                    return;
                }

                Transform equipmentAreaWindowTransform = inventoryObj.GetComponentInChildren<vEquipArea>(true).gameObject.transform.parent;

                // EquipMentArea_Builder
                var equipMentAreaBuilderTransform = equipmentAreaWindowTransform.Find("EquipMentArea_Builder");

                GameObject equipMentAreaBuilderObj;
                if (equipMentAreaBuilderTransform == null)
                {
                    GameObject equipMentAreaBuilderPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(MISEditor.MIS_ASSETS_PATH, "Add-ons/Builder/Prefabs/EquipMentArea_Builder.prefab"));
                    equipMentAreaBuilderObj = equipMentAreaBuilderPrefab.Instantiate3D(Vector3.zero, equipmentAreaWindowTransform);
                    equipMentAreaBuilderObj.name = "EquipMentArea_Builder";
                }
                else
                {
                    equipMentAreaBuilderObj = equipMentAreaBuilderTransform.gameObject;
                }
                vEquipArea builderEquipArea = equipMentAreaBuilderObj.GetComponent<vEquipArea>();

                // EquipmentPickerWindow
                vItemWindow equipmentPickerWindow = equipmentAreaWindowTransform.parent.parent.gameObject.GetComponentInChildren<vItemWindow>(true);

                if (equipmentPickerWindow == null)
                {
                    Debug.LogError("[INVECTOR Builder]PickerWindow cannot be found. Please remove Inventory root object then run Setup again.");
                    return;
                }

                builderEquipArea.itemPicker = equipmentPickerWindow;

                // EquipmentDisplayWindow
                var equipmentDisplayWindowTransform = inventoryObj.transform.Find("EquipmentDisplayWindow");

                if (equipmentDisplayWindowTransform == null)
                {
                    Debug.LogError("[INVECTOR Builder]EquipmentDisplayWindow cannot be found. Please remove Inventory root object then run Setup again.");
                    return;
                }

                // EquipDisplay_Up
                var equipDisplayUpTransform = equipmentDisplayWindowTransform.Find("EquipDisplay_Up");

                GameObject equipDisplayUpObj;
                if (equipDisplayUpTransform == null)
                {
                    GameObject equipDisplayUpPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(MISEditor.MIS_ASSETS_PATH, "Add-ons/Builder/Prefabs/EquipDisplay_Up.prefab"));
                    equipDisplayUpObj = equipDisplayUpPrefab.Instantiate2D(new Vector3(0, 80, 0), equipmentDisplayWindowTransform);
                    equipDisplayUpObj.name = "EquipDisplay_Up";
                }
                else
                {
                    equipDisplayUpObj = equipDisplayUpTransform.gameObject;
                }
                vEquipmentDisplay equipDisplayUp = equipDisplayUpObj.GetComponent<vEquipmentDisplay>();

                // changeEquipmentControllers
                if (inventory.changeEquipmentControllers == null)
                    inventory.changeEquipmentControllers = new List<ChangeEquipmentControl>();

                ChangeEquipmentControl builderEquipmentControl = inventory.changeEquipmentControllers.Find(x => x.equipArea.Equals(builderEquipArea));

                if (builderEquipmentControl == null)
                {
                    if (inventory.changeEquipmentControllers.Find(x => x.display.Equals(equipDisplayUp)) == null)
                    {
                        // Add new
                        builderEquipmentControl = new ChangeEquipmentControl();

                        builderEquipmentControl.useItemInput = new GenericInput("U", "LT", "LT");
                        builderEquipmentControl.useItemInput.useInput = false;

                        builderEquipmentControl.previousItemInput = new GenericInput("UpArrow", "D-Pad Vertical", "D-Pad Vertical");
                        builderEquipmentControl.previousItemInput.useInput = true;

                        builderEquipmentControl.nextItemInput = new GenericInput("Horizontal", "Horizontal", "Horizontal");
                        builderEquipmentControl.nextItemInput.useInput = false;

                        builderEquipmentControl.equipArea = builderEquipArea;
                        builderEquipmentControl.display = equipDisplayUp;

                        inventory.changeEquipmentControllers.Add(builderEquipmentControl);
                    }
                    else
                    {
                        builderEquipmentControl = inventory.changeEquipmentControllers.Find(x => x.display.Equals(equipDisplayUp));

                        builderEquipmentControl.useItemInput.useInput = false;
                        builderEquipmentControl.previousItemInput.useInput = true;
                        builderEquipmentControl.nextItemInput.useInput = false;

                        builderEquipmentControl.equipArea = builderEquipArea;
                    }
                }
                else
                {
                    builderEquipmentControl.equipArea = builderEquipArea;
                    builderEquipmentControl.display = equipDisplayUp;
                }
            }


            // ----------------------------------------------------------------------------------------------------
            // Animator
            // ----------------------------------------------------------------------------------------------------
            BuilderParameters();
            BuilderBaseLayer();
            BuilderAnimatorTransitions();
            BuilderPosition();
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void BuilderParameters()
        {
            // Do nothing
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void BuilderBaseLayer()
        {
            // ----------------------------------------------------------------------------------------------------
            // Animation Clips
            // ----------------------------------------------------------------------------------------------------
            List<AnimationClip> builderSetClipList = GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_BUILDER_PATH, "Animations/vBuilderSet.fbx"));
            var hammeringWoodClip = builderSetClipList.FindClip("HammeringWood");
            var hammeringWood_downClip = builderSetClipList.FindClip("HammeringWood_down");
            var fixing_crouchClip = builderSetClipList.FindClip("Fixing_crouch");
            var healingClip = builderSetClipList.FindClip("Healing");


            // ----------------------------------------------------------------------------------------------------
            // Base - Action
            // ----------------------------------------------------------------------------------------------------


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions - Builder
            base_BuilderSM = base_ActionsSM.CreateStateMachineIfNotExist(MISFeature.MIS_PACKAGE_V_BUILDER);


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions - Builder - Planting Mine
            base_PlantingMine = base_BuilderSM.CreateStateIfNotExist("Planting Mine", fixing_crouchClip, true);

            // vAnimatorTag
            if (!base_PlantingMine.TryGetStateMachineBehaviour(out vAnimatorTag base_PlantingMineAnimatorTag))
                base_PlantingMineAnimatorTag = base_PlantingMine.AddStateMachineBehaviour<vAnimatorTag>();

            base_PlantingMineAnimatorTag.tags = base_PlantingMineAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            base_PlantingMineAnimatorTag.tags = base_PlantingMineAnimatorTag.tags.AddStringIfNotExist(TAG_CUSTOM_ACTION);


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions - Builder - HammeringWood
            base_HammeringWood = base_BuilderSM.CreateStateIfNotExist("HammeringWood", hammeringWoodClip, true);

            // vAnimatorTag
            if (!base_HammeringWood.TryGetStateMachineBehaviour(out vAnimatorTag base_HammeringWoodAnimatorTag))
                base_HammeringWoodAnimatorTag = base_HammeringWood.AddStateMachineBehaviour<vAnimatorTag>();

            base_HammeringWoodAnimatorTag.tags = base_HammeringWoodAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            base_HammeringWoodAnimatorTag.tags = base_HammeringWoodAnimatorTag.tags.AddStringIfNotExist(TAG_CUSTOM_ACTION);

            // vAnimatorEvent
            if (!base_HammeringWood.TryGetStateMachineBehaviour(out vAnimatorEvent base_HammeringWoodAnimatorEvent))
                base_HammeringWoodAnimatorEvent = base_HammeringWood.AddStateMachineBehaviour<vAnimatorEvent>();

            vAnimatorEventTrigger base_HammeringWoodEnableHammerAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "EnableHammer",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.EnterState
            };
            vAnimatorEventTrigger base_HammeringWoodDisableHammerAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "DisableHammer",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.ExitState
            };

            if (base_HammeringWoodAnimatorEvent.eventTriggers == null)
                base_HammeringWoodAnimatorEvent.eventTriggers = new List<vAnimatorEventTrigger>();

            if (base_HammeringWoodAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(base_HammeringWoodEnableHammerAnimatorEventTrigger.eventName)) == null)
                base_HammeringWoodAnimatorEvent.eventTriggers.Add(base_HammeringWoodEnableHammerAnimatorEventTrigger);

            if (base_HammeringWoodAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(base_HammeringWoodDisableHammerAnimatorEventTrigger.eventName)) == null)
                base_HammeringWoodAnimatorEvent.eventTriggers.Add(base_HammeringWoodDisableHammerAnimatorEventTrigger);


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions - Builder - Healing
            base_Healing = base_BuilderSM.CreateStateIfNotExist("Healing", healingClip);

            // vAnimatorTag
            if (!base_Healing.TryGetStateMachineBehaviour(out vAnimatorTag base_HealingAnimatorTag))
                base_HealingAnimatorTag = base_Healing.AddStateMachineBehaviour<vAnimatorTag>();

            base_HealingAnimatorTag.tags = base_HealingAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            base_HealingAnimatorTag.tags = base_HealingAnimatorTag.tags.AddStringIfNotExist(TAG_CUSTOM_ACTION);

            // vAnimatorEvent
            if (!base_Healing.TryGetStateMachineBehaviour(out vAnimatorEvent base_HealingAnimatorEvent))
                base_HealingAnimatorEvent = base_Healing.AddStateMachineBehaviour<vAnimatorEvent>();

            vAnimatorEventTrigger base_HealingHealingAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "Healing",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.EnterState
            };

            if (base_HealingAnimatorEvent.eventTriggers == null)
                base_HealingAnimatorEvent.eventTriggers = new List<vAnimatorEventTrigger>();

            if (base_HealingAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(base_HealingHealingAnimatorEventTrigger.eventName)) == null)
                base_HealingAnimatorEvent.eventTriggers.Add(base_HealingHealingAnimatorEventTrigger);


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions - Builder - HammeringTrap
            base_HammeringTrap = base_BuilderSM.CreateStateIfNotExist("HammeringTrap", hammeringWood_downClip, true);

            // vAnimatorTag
            if (!base_HammeringTrap.TryGetStateMachineBehaviour(out vAnimatorTag base_HammeringTrapAnimatorTag))
                base_HammeringTrapAnimatorTag = base_HammeringTrap.AddStateMachineBehaviour<vAnimatorTag>();

            base_HammeringTrapAnimatorTag.tags = base_HammeringTrapAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            base_HammeringTrapAnimatorTag.tags = base_HammeringTrapAnimatorTag.tags.AddStringIfNotExist(TAG_CUSTOM_ACTION);

            // vAnimatorEvent
            if (!base_HammeringTrap.TryGetStateMachineBehaviour(out vAnimatorEvent base_HammeringTrapAnimatorEvent))
                base_HammeringTrapAnimatorEvent = base_HammeringTrap.AddStateMachineBehaviour<vAnimatorEvent>();

            vAnimatorEventTrigger base_HammeringTrapEnableHammerAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "EnableHammer",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.EnterState
            };
            vAnimatorEventTrigger base_HammeringTrapDisableHammerAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "DisableHammer",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.ExitState
            };

            if (base_HammeringTrapAnimatorEvent.eventTriggers == null)
                base_HammeringTrapAnimatorEvent.eventTriggers = new List<vAnimatorEventTrigger>();

            if (base_HammeringTrapAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(base_HammeringTrapEnableHammerAnimatorEventTrigger.eventName)) == null)
                base_HammeringTrapAnimatorEvent.eventTriggers.Add(base_HammeringTrapEnableHammerAnimatorEventTrigger);

            if (base_HammeringTrapAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(base_HammeringTrapDisableHammerAnimatorEventTrigger.eventName)) == null)
                base_HammeringTrapAnimatorEvent.eventTriggers.Add(base_HammeringTrapDisableHammerAnimatorEventTrigger);


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions - Builder - Building Crouched
            base_BuildingCrouched = base_BuilderSM.CreateStateIfNotExist("Building Crouched", fixing_crouchClip, true);

            // vAnimatorTag
            if (!base_BuildingCrouched.TryGetStateMachineBehaviour(out vAnimatorTag base_BuildingCrouchedAnimatorTag))
                base_BuildingCrouchedAnimatorTag = base_BuildingCrouched.AddStateMachineBehaviour<vAnimatorTag>();

            base_BuildingCrouchedAnimatorTag.tags = base_BuildingCrouchedAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            base_BuildingCrouchedAnimatorTag.tags = base_BuildingCrouchedAnimatorTag.tags.AddStringIfNotExist(TAG_CUSTOM_ACTION);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void BuilderAnimatorTransitions()
        {
            // ----------------------------------------------------------------------------------------------------
            // Base - Action
            // ----------------------------------------------------------------------------------------------------

            // Builder to Exit
            base_ActionsSM.AddExitTransitionIfNotExist(base_BuilderSM, null);


            // PlantingMine
            base_PlantingMine.AddExitTransitionIfNotExist(null, true, 0.44f, true, 0.28f);

            // HammeringWood
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_ACTIONSTATE, AnimatorConditionMode.Equals, 0f));
            base_HammeringWood.AddExitTransitionIfNotExist(conditionList, false, 0.76f, true, 0.1f);

            // Healing
            base_Healing.AddExitTransitionIfNotExist(null, true, 0.93f);

            // HammeringTrap
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_ACTIONSTATE, AnimatorConditionMode.Equals, 0f));
            base_HammeringTrap.AddExitTransitionIfNotExist(conditionList, false, 0.42f);

            // BuildingCrouched To Exit
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_ACTIONSTATE, AnimatorConditionMode.Equals, 0f));
            base_BuildingCrouched.AddExitTransitionIfNotExist(conditionList, false, 0.58f);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void BuilderPosition()
        {
            // ----------------------------------------------------------------------------------------------------
            // Base Layer
            // ----------------------------------------------------------------------------------------------------


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions
            base_ActionsSM.SetStateMachineRelativePosition(base_BuilderSM, 10, 0);


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions - Builder
            base_BuilderSM.SetDefaultLayerAllPosition();
            base_BuilderSM.ArrangeStates(0);
        }
    }
}