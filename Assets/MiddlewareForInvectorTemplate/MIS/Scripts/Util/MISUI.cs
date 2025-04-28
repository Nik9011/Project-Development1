using UnityEngine;
using UnityEngine.UI;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public static class MISUI
    {
        // ----------------------------------------------------------------------------------------------------
        // Image
        // ----------------------------------------------------------------------------------------------------
        public static void SetSprite(this Image image, Sprite sprite)
        {
            if (image == null)
                return;

            image.sprite = sprite;
        }
        public static void SetSprite(this Image image, Sprite sprite, float alpha)
        {
            if (image == null)
                return;

            Color color = image.color;
            image.sprite = sprite;
            color.a = alpha;
            image.color = color;
        }
        public static void SetSprite(this Image image, Sprite sprite, Color newColor, float alpha = 1f)
        {
            if (image == null)
                return;

            Color color = newColor;
            image.sprite = sprite;
            color.a = alpha;
            image.color = color;
        }
        public static void ClearSprite(this Image image)
        {
            Color color = image.color;
            image.sprite = null;
            color.a = 0f;
            image.color = color;
        }

        // ----------------------------------------------------------------------------------------------------
        // Image
        // ----------------------------------------------------------------------------------------------------
        public static void SetAlpha(this Image image, float alpha)
        {
            Color color = image.color;

            if (color.a != alpha)
            {
                color.a = alpha;
                image.color = color;
            }
        }
        public static void SetAlpha(this RawImage image, float alpha)
        {
            Color color = image.color;

            if (color.a != alpha)
            {
                color.a = alpha;
                image.color = color;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // Image
        // ----------------------------------------------------------------------------------------------------
        public static void SetColor(this Image image, Color newColor)
        {
            image.color = newColor;
        }

        // ----------------------------------------------------------------------------------------------------
        // Text
        // ----------------------------------------------------------------------------------------------------
        public static void SetColor(this Text text, Color color)
        {
            text.color = color;
        }
        public static void SetAlpha(this Text text, float alpha)
        {
            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }
    }
}