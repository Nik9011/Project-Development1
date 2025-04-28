using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public interface IMISColliderFilter
    {
        bool FilterCollider(Collider other);
    }
}