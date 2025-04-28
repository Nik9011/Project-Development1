using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public enum mv8WayType
    {
        None = 0,

        NorthEast = 1,
        East,
        SouthEast = 3,
        South,
        SouthWest = 5,
        West,
        NorthWest = 7,
        North
    }

    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public static class EightWayTypeExtension
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static mv8WayType Get8WayType(this Vector2 input)
        {
            if (input == Vector2.zero)
                return mv8WayType.None;

            return input.Translate8WayType();
        }
        public static mv8WayType Get8WayType(this Vector3 input)
        {
            if (input == Vector3.zero)
                return mv8WayType.None;

            Vector2 newInput = new Vector2(input.x, input.z);
            return newInput.Translate8WayType();
        }
        
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static Vector2 ClampInput(this Vector2 input)
        {
            if (input.x > 0)
                input.x = 1f;
            if (input.x < 0)
                input.x = -1f;

            if (input.y > 0)
                input.y = 1f;
            if (input.y < 0)
                input.y = -1f;

            return input;
        }
        public static Vector2 ClampInput(this Vector3 input)
        {
            if (input.x > 0)
                input.x = 1f;
            if (input.x < 0)
                input.x = -1f;

            if (input.z > 0)
                input.z = 1f;
            if (input.z < 0)
                input.z = -1f;

            return new Vector2(input.x, input.z);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        private static mv8WayType Translate8WayType(this Vector2 input)
        {
            if (input.x >= 0.5f && input.y >= 0.5f)
                return mv8WayType.NorthEast;
            else if (input.x >= 0.5f && input.y > -0.25f && input.y < 0.25f)
                return mv8WayType.East;
            else if (input.x >= 0.5f && input.y <= -0.5f)
                return mv8WayType.SouthEast;
            else if (input.x > -0.25f && input.x < 0.25f && input.y <= -0.5f)
                return mv8WayType.South;
            else if (input.x <= -0.5f && input.y <= -0.5f)
                return mv8WayType.SouthWest;
            else if (input.x <= -0.5f && input.y > -0.25f && input.y < 0.25f)
                return mv8WayType.West;
            else if (input.x <= -0.5f && input.y >= 0.5f)
                return mv8WayType.NorthWest;
            else if (input.x > -0.25f && input.x < 0.25f && input.y >= 0.5f)
                return mv8WayType.North;
            else
                return mv8WayType.None;
        }
    }
}