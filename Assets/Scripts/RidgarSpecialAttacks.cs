using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RidgarSpecialAttacks : MonoBehaviour
{
    [Header("Roar Attack Settings")]
    public float cooldownDuration = 5f;
    public Button roarButton;
    public GameObject roarVFX;
    public Transform vfxSpawnPoint;
    public float vfxDelay = 0.6f;
    public AudioClip roarAudio;

    [Header("Spin Attack Settings")]
    public Button spinButton;
    public float spinCooldown = 5f;
    public float spinMoveSpeed = 5f;
    public float spinMoveDuration = 0.5f;
    public AudioClip spinAudio;

    [Header("Slam Attack Settings")]
    public Button slamButton;
    public GameObject slamVFX;
    public float slamCooldown = 5f;
    public float slamVFXDelay = 0.6f;
    public Transform SlamSpawnPoint;
    public AudioClip slamAudio;

    [Header("Ultimate Attack Settings")]
    public Button ultimateButton;
    public GameObject ultimateVFX;
    public float ultimateCooldown = 10f;
    public float ultimateVFXDelay = 0.7f;
    public AudioClip ultimateAudio;
    public Transform UltimateSpawnPoint;
    private Animator animator;
    private Rigidbody rb;

    private bool isOnCooldown = false;
    private bool isSpinCooldown = false;
    private bool isSlamCooldown = false;
    private bool isUltimateCooldown = false;
    public Animator anim;
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (animator == null) Debug.LogWarning("Animator not found.");
        if (rb == null) Debug.LogWarning("Rigidbody not found.");

        if (roarButton != null) roarButton.onClick.AddListener(UseRoarAttack);
        if (spinButton != null) spinButton.onClick.AddListener(UseSpinAttack);
        if (slamButton != null) slamButton.onClick.AddListener(UseSlamAttack);
        if (ultimateButton != null) ultimateButton.onClick.AddListener(UseUltimateAttack);
    }

    // ------------------ ROAR ATTACK ------------------
    public void UseRoarAttack()
    {
        if (isOnCooldown) return;

        animator.Play("Roar");
        if (roarAudio != null)
            AudioSource.PlayClipAtPoint(roarAudio, transform.position);

        StartCoroutine(TriggerRoarVFX(vfxDelay));
        StartCoroutine(RoarCooldownRoutine());
    }

    private IEnumerator TriggerRoarVFX(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (roarVFX != null && vfxSpawnPoint != null)
            Instantiate(roarVFX, vfxSpawnPoint.position, vfxSpawnPoint.rotation);

        yield return new WaitForSeconds(3);
        anim.speed = 1.5f;
        yield return new WaitForSeconds(5);
        anim.speed = 1;
    }

    private IEnumerator RoarCooldownRoutine()
    {
        isOnCooldown = true;
        roarButton.interactable = false;

        yield return new WaitForSeconds(cooldownDuration);

        isOnCooldown = false;
        roarButton.interactable = true;
     
    }

    // ------------------ SPIN ATTACK ------------------
    public void UseSpinAttack()
    {
        if (isSpinCooldown) return;

        animator.Play("spinning");

        if (spinAudio != null)
            AudioSource.PlayClipAtPoint(spinAudio, transform.position);

        StartCoroutine(SpinForwardMovement());
        StartCoroutine(SpinCooldownRoutine());
    }

    private IEnumerator SpinForwardMovement()
    {
        yield return new WaitForSeconds(1);
        float timer = 0f;
        while (timer < spinMoveDuration)
        {
            transform.Translate(Vector3.forward * spinMoveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SpinCooldownRoutine()
    {
        isSpinCooldown = true;
        spinButton.interactable = false;

        yield return new WaitForSeconds(spinCooldown);

        isSpinCooldown = false;
        spinButton.interactable = true;
    }

    // ------------------ SLAM ATTACK ------------------
    public void UseSlamAttack()
    {
        if (isSlamCooldown) return;

        animator.Play("GroundSlam");

        if (slamAudio != null)
            AudioSource.PlayClipAtPoint(slamAudio, transform.position);

        //StartCoroutine(SlamJumpThenVFX());
        StartCoroutine(SlamCooldownRoutine());
    }

    //private IEnumerator SlamJumpThenVFX()
    //{
    //    yield return new WaitForSeconds(slamJumpDelay);

    //    if (rb != null)
    //        rb.AddForce(Vector3.up * slamJumpForce, ForceMode.Impulse);

    //    yield return new WaitForSeconds(slamVFXDelay);

    //    
    //}

    private IEnumerator SlamCooldownRoutine()
    {
        isSlamCooldown = true;
        slamButton.interactable = false;
        yield return new WaitForSeconds(1);
        if (slamVFX != null && SlamSpawnPoint != null)
              Instantiate(slamVFX, SlamSpawnPoint.position, SlamSpawnPoint.rotation);

            yield return new WaitForSeconds(slamCooldown);

        isSlamCooldown = false;
        slamButton.interactable = true;
    }

    // ------------------ ULTIMATE ATTACK ------------------
    public void UseUltimateAttack()
    {
        if (isUltimateCooldown) return;

        animator.Play("UltimateImpact");

        if (ultimateAudio != null)
            AudioSource.PlayClipAtPoint(ultimateAudio, transform.position);

        StartCoroutine(UltimateVFXRoutine());
        StartCoroutine(UltimateCooldownRoutine());
    }

    private IEnumerator UltimateVFXRoutine()
    {
        yield return new WaitForSeconds(ultimateVFXDelay);

        if (ultimateVFX != null && UltimateSpawnPoint != null)
            Instantiate(ultimateVFX, UltimateSpawnPoint.position, UltimateSpawnPoint.rotation);
    }

    private IEnumerator UltimateCooldownRoutine()
    {
        isUltimateCooldown = true;
        ultimateButton.interactable = false;

        yield return new WaitForSeconds(ultimateCooldown);

        isUltimateCooldown = false;
        ultimateButton.interactable = true;
    }
}
