#if INVECTOR_MELEE || INVECTOR_SHOOTER
using Invector;
using Invector.vItemManager;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("Spawn Effect", iconName = "misIconRed")]
    public partial class mvEquipAreaControl : vMonoBehaviour
    {
        public vEquipArea area;

        void Start()
        {
            area.onPickUpItemCallBack = OnPickUpItemCallBack;

            vInventory inventory = GetComponentInParent<vInventory>();
            if (inventory)
                inventory.onOpenCloseInventory.AddListener(OnOpen);
        }

        public virtual void OnOpen(bool value)
        {
        }

        public virtual void OnPickUpItemCallBack(vEquipArea area, vItemSlot slot)
        {
            var sameSlots = area.equipSlots.FindAll(slotInArea => slotInArea != slot && slotInArea.item != null && (slotInArea.item.id == slot.item.id));

            for (int i = 0; i < sameSlots.Count; i++)
                area.RemoveItemOfEquipSlot(sameSlots[i]);
        }
    }
}
#endif