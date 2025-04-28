using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Invector.vCharacterController;

public class Magdor_SpecialAttack : MonoBehaviour
{
    [Header("Dragon Slash Settings")]
    public Button dragonSlashButton;
    public GameObject dragonSlashVFX;
    public Transform dragonSlashVFXSpawnPoint;
    public float dragonSlashVFXDelay = 0.5f;
    public float dragonSlashCooldownDuration = 5f;
    public AudioClip dragonSlashAudio;

    [Header("Dash Attack Settings")]
    public Button dashAttackButton;
    public GameObject dashStartVFX;  // First VFX (e.g., dust effect)
    public GameObject dashTrailVFX;  // Second VFX (e.g., slash trail effect)
    public Transform dashVFXSpawnPoint;
    public float dashStartVFXDelay = 0f;  // When to spawn the first VFX
    public float dashTrailVFXDelay = 0.15f;  // When to spawn the second VFX
    public float dashForce = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldownDuration = 4f;
    public AudioClip dashAttackAudio;

    [Header("New Single VFX Attack Settings")]
    public Button newAttackButton;
    public GameObject newAttackVFX;  // Single VFX for this attack
    public Transform newAttackVFXSpawnPoint;
    public float newAttackCooldownDuration = 3f;
    public AudioClip newAttackAudio;
    public float newAttackVFXDelay = 0.5f;  // Delay for VFX in the new attack

    [Header("Ultimate Attack Settings")]
    public Button ultimateAttackButton;
    public GameObject ultimateAttackVFX;  // VFX for the ultimate attack
    public Transform ultimateAttackVFXSpawnPoint;
    public float ultimateAttackCooldownDuration = 10f;
    public AudioClip ultimateAttackAudio;
    public float ultimateAttackVFXDelay = 1f;  // Delay for VFX in the ultimate attack

    private Animator animator;
    private bool isDragonSlashOnCooldown = false;
    private bool isDashOnCooldown = false;
    private bool isNewAttackOnCooldown = false;
    private bool isUltimateAttackOnCooldown = false;

    private Rigidbody rb;
    private bool isDashing = false;
    public vShooterMeleeInput controller;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (animator == null) Debug.LogWarning("Animator not found.");
        if (rb == null) Debug.LogWarning("Rigidbody not found.");

        if (dragonSlashButton != null)
            dragonSlashButton.onClick.AddListener(UseDragonSlash);

        if (dashAttackButton != null)
            dashAttackButton.onClick.AddListener(UseDashAttack);

        if (newAttackButton != null)
            newAttackButton.onClick.AddListener(UseNewAttack);

        if (ultimateAttackButton != null)
            ultimateAttackButton.onClick.AddListener(UseUltimateAttack);
    }

    // ====== Dragon Slash ======
    public void UseDragonSlash()
    {
        if (isDragonSlashOnCooldown) return;

        animator.Play("DragonSlash");

        if (dragonSlashAudio != null)
            AudioSource.PlayClipAtPoint(dragonSlashAudio, transform.position);

        StartCoroutine(TriggerDragonSlashVFX(dragonSlashVFXDelay));
        StartCoroutine(CooldownRoutine(dragonSlashButton, dragonSlashCooldownDuration, () => isDragonSlashOnCooldown = false));
        isDragonSlashOnCooldown = true;
    }

    private IEnumerator TriggerDragonSlashVFX(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (dragonSlashVFX != null && dragonSlashVFXSpawnPoint != null)
            Instantiate(dragonSlashVFX, dragonSlashVFXSpawnPoint.position, transform.rotation);
    }

    // ====== Dash Attack ======
    public void UseDashAttack()
    {
        if (isDashOnCooldown) return;

        animator.Play("DashAttack");

        if (dashAttackAudio != null)
            AudioSource.PlayClipAtPoint(dashAttackAudio, transform.position);

        // Trigger both VFX with delays
        StartCoroutine(TriggerDashVFX(dashStartVFX, dashStartVFXDelay)); // First VFX
        StartCoroutine(TriggerDashVFX(dashTrailVFX, dashTrailVFXDelay)); // Second VFX
        rb.useGravity = false;
        StartCoroutine(DashForward());
        StartCoroutine(CooldownRoutine(dashAttackButton, dashCooldownDuration, () => isDashOnCooldown = false));
        isDashOnCooldown = true;
    }

    private IEnumerator TriggerDashVFX(GameObject vfxPrefab, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (vfxPrefab != null && dashVFXSpawnPoint != null)
            Instantiate(vfxPrefab, dashVFXSpawnPoint.position, transform.rotation);
    }

    private IEnumerator DashForward()
    {
        yield return new WaitForSeconds(1);
        isDashing = true;
        controller.enabled = false;
        rb.useGravity = false;
        if (rb != null)
        {
            rb.velocity = transform.forward * dashForce;
        }

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector3.zero;
        controller.enabled = true;
        rb.useGravity = true;
        isDashing = false;
    }

    // ====== New Attack with Single VFX ======
    public void UseNewAttack()
    {
        if (isNewAttackOnCooldown) return;

        animator.Play("Cross Slash");

        if (newAttackAudio != null)
            AudioSource.PlayClipAtPoint(newAttackAudio, transform.position);

        // Trigger the single VFX with delay
        StartCoroutine(TriggerNewAttackVFX());
        StartCoroutine(CooldownRoutine(newAttackButton, newAttackCooldownDuration, () => isNewAttackOnCooldown = false));
        isNewAttackOnCooldown = true;
    }

    private IEnumerator TriggerNewAttackVFX()
    {
        yield return new WaitForSeconds(newAttackVFXDelay);  // Delay before triggering VFX
        if (newAttackVFX != null && newAttackVFXSpawnPoint != null)
            Instantiate(newAttackVFX, newAttackVFXSpawnPoint.position, transform.rotation);
    }

    // ====== Ultimate Attack with VFX ======
    public void UseUltimateAttack()
    {
        if (isUltimateAttackOnCooldown) return;

        animator.Play("UltimateAttack");

        if (ultimateAttackAudio != null)
            AudioSource.PlayClipAtPoint(ultimateAttackAudio, transform.position);

        // Trigger the VFX with delay
        StartCoroutine(TriggerUltimateAttackVFX());
        StartCoroutine(CooldownRoutine(ultimateAttackButton, ultimateAttackCooldownDuration, () => isUltimateAttackOnCooldown = false));
        isUltimateAttackOnCooldown = true;
    }

    private IEnumerator TriggerUltimateAttackVFX()
    {
        yield return new WaitForSeconds(ultimateAttackVFXDelay);  // Delay before triggering VFX
        if (ultimateAttackVFX != null && ultimateAttackVFXSpawnPoint != null)
            Instantiate(ultimateAttackVFX, ultimateAttackVFXSpawnPoint.position, transform.rotation);
    }

    // ====== Cooldown Utility ======
    private IEnumerator CooldownRoutine(Button button, float duration, System.Action onCooldownEnd)
    {
        if (button != null)
        {
            button.interactable = false;
            button.GetComponent<Image>().raycastTarget = false;
        }

        yield return new WaitForSeconds(duration);

        if (button != null)
        {
            button.interactable = true;
            button.GetComponent<Image>().raycastTarget = true;
        }

        onCooldownEnd?.Invoke();
    }
}
