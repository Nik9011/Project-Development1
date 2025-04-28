using Invector;
using Invector.vCharacterController;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("Demo Scene", iconName = "misIconRed")]
    public class mvDemoScene : vMonoBehaviour
    {
#if MIS
        [Header("Scene")]
        [SerializeField] protected GenericInput quitInput = new GenericInput("Escape", "", "");
        [SerializeField] protected GenericInput restartInput = new GenericInput("Insert", "", "");
        [SerializeField] protected GenericInput debugInput = new GenericInput("Delete", "", "");

        [Header("Time Scale")]
        [SerializeField] protected mvFloatOrigin newTimeScale = new mvFloatOrigin(0.2f, 0.2f);
        [SerializeField] protected float timeScaleStep = 0.2f;
        [SerializeField] protected GenericInput setTimScaleInput = new GenericInput("End", "", "");
        [SerializeField] protected GenericInput setTimScaleUpInput = new GenericInput("PageUp", "", "");
        [SerializeField] protected GenericInput setTimScaleDownInput = new GenericInput("PageDown", "", "");

        [Header("Character")]
        [vHelpBox("Freeze Animator Input would be useful during Shooter IK Adjust")]
        [SerializeField] protected GenericInput freezeAnimatorInput = new GenericInput("Backspace", "", "");
        [SerializeField] protected GenericInput killInput = new GenericInput("Home", "", "");
        public vHealthController character;
        protected bool isFreezedAnimator;
        protected float lastAnimatorSpeed;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void Update()
        {
            // Quit
            if (quitInput.useInput && quitInput.GetButtonDown())
            {
                if (Application.isPlaying)
                {
#if UNITY_EDITOR
                    EditorApplication.ExitPlaymode();
#else
                    Application.Quit();
#endif
                }
            }


            // Restart scene
            if (restartInput.useInput && restartInput.GetButtonDown())
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);


            // Debug break
            if (debugInput.useInput && debugInput.GetButtonDown())
                Debug.Break();


            // Time scale
            if (setTimScaleInput.useInput && setTimScaleInput.GetButtonDown())
                Time.timeScale = newTimeScale.now;

            if (setTimScaleUpInput.useInput && setTimScaleUpInput.GetButtonDown())
            {
                newTimeScale.now += timeScaleStep;
                if (newTimeScale.now > 1f)
                    newTimeScale.now = 1f;

                Time.timeScale = newTimeScale.now;
            }
            if (setTimScaleDownInput.useInput && setTimScaleDownInput.GetButtonDown())
            {
                newTimeScale.now -= timeScaleStep;
                if (newTimeScale.now < 0f)
                    newTimeScale.now = 0f;

                Time.timeScale = newTimeScale.now;
            }


            if (character != null)
            {
                // Freeze Animator
                if (freezeAnimatorInput.useInput && freezeAnimatorInput.GetButtonDown())
                {
                    if (character.gameObject.TryGetComponent(out Animator animator))
                    {
                        if (isFreezedAnimator)
                        {
                            isFreezedAnimator = false;
                            animator.speed = lastAnimatorSpeed;
                        }
                        else
                        {
                            isFreezedAnimator = true;
                            lastAnimatorSpeed = animator.speed;
                            animator.speed = 0;
                        }
                    }
                }


                // Kill a character
                if (killInput.useInput && killInput.GetButtonDown())
                    character.TakeDamage(new vDamage(1000000));
            }
        }
#endif
    }
}