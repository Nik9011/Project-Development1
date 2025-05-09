using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public class mvScrollRect : ScrollRect
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        [Header("MIS")]
        public bool useDrag = true;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public override void OnBeginDrag(PointerEventData ped)
        {
            if (!useDrag)
                return;

            base.OnBeginDrag(ped);
        }
    }
}