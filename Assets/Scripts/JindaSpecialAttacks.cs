using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JindaSpecialAttacks : MonoBehaviour
{
    [Header("Attack One Settings")]
    public Button attackOneButton;
    public GameObject attackOneVFX, arrowEffect;
    public Transform vfxSpawnPoint, arrowSpawnPoint;
    public float vfxDelay = 0.5f;
    public float cooldownDuration = 5f;
    public AudioClip attackOneAudio;

    [Header("Trap Attack Settings")]
    public Button trapAttackButton;
    public GameObject trapVFX;
    public Transform trapSpawnPoint;
    public float trapThrowForce = 10f;
    public float trapCooldownDuration = 7f;
    public float trapVFXDelay = 0.3f;
    public AudioClip trapAttackAudio;

    [Header("Ultimate Attack Settings")]
    public Button ultimateAttackButton;
    public GameObject ultimateVFX;
    public Transform ultimateVFXSpawnPoint;
    public float ultimateDelay = 0.6f;
    public float ultimateCooldown = 8f;
    public AudioClip ultimateAudio;

    private Animator animator;
    private bool isAttackOneOnCooldown = false;
    private bool isTrapOnCooldown = false;
    private bool isUltimateOnCooldown = false;

    private GameObject arrowSpawned;
    public CameraShake shake;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogWarning("Animator not found.");

        if (attackOneButton != null)
            attackOneButton.onClick.AddListener(UseAttackOne);

        if (trapAttackButton != null)
            trapAttackButton.onClick.AddListener(UseTrapAttack);

        if (ultimateAttackButton != null)
            ultimateAttackButton.onClick.AddListener(UseUltimateAttack);
    }

    // ========== ATTACK ONE ==========
    public void UseAttackOne()
    {
        if (isAttackOneOnCooldown) return;

        animator.Play("Ability1");

        if (attackOneAudio != null)
            AudioSource.PlayClipAtPoint(attackOneAudio, transform.position);

        InitiateVfx();
        StartCoroutine(TriggerAttackOneVFX(vfxDelay));
        StartCoroutine(CooldownRoutine(attackOneButton, cooldownDuration, () => isAttackOneOnCooldown = false));
        isAttackOneOnCooldown = true;
    }

    private IEnumerator TriggerAttackOneVFX(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (attackOneVFX != null && vfxSpawnPoint != null)
            Instantiate(attackOneVFX, vfxSpawnPoint.position, transform.rotation);
    }

    public void InitiateVfx()
    {
        arrowSpawned = Instantiate(arrowEffect, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        Invoke(nameof(DestroyArrow), 2f);
    }

    private void DestroyArrow()
    {
        if (arrowSpawned != null)
            Destroy(arrowSpawned);
    }

    // ========== TRAP ATTACK ==========
    public void UseTrapAttack()
    {
        if (isTrapOnCooldown) return;

        animator.Play("AbilityTrap"); // Replace with your actual animation name

        if (trapAttackAudio != null)
            AudioSource.PlayClipAtPoint(trapAttackAudio, transform.position);

        StartCoroutine(TriggerTrap(trapVFXDelay));
        StartCoroutine(CooldownRoutine(trapAttackButton, trapCooldownDuration, () => isTrapOnCooldown = false));
        isTrapOnCooldown = true;
    }

    private IEnumerator TriggerTrap(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (trapVFX != null && trapSpawnPoint != null)
        {
            GameObject trap = Instantiate(trapVFX, trapSpawnPoint.position, transform.rotation);
            Rigidbody rb = trap.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 forceDirection = transform.forward + transform.up * 0.5f;
                rb.AddForce(forceDirection.normalized * trapThrowForce, ForceMode.Impulse);
            }
        }
    }

    // ========== ULTIMATE ATTACK ==========
    public void UseUltimateAttack()
    {
        if (isUltimateOnCooldown) return;

        animator.Play("UltimateAttack"); // Replace with your ultimate animation name

        if (ultimateAudio != null)
            AudioSource.PlayClipAtPoint(ultimateAudio, transform.position);

        StartCoroutine(TriggerUltimateVFX(ultimateDelay));
        StartCoroutine(CooldownRoutine(ultimateAttackButton, ultimateCooldown, () => isUltimateOnCooldown = false));
        isUltimateOnCooldown = true;
    }

    private IEnumerator TriggerUltimateVFX(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (ultimateVFX != null && ultimateVFXSpawnPoint != null)
            Instantiate(ultimateVFX, ultimateVFXSpawnPoint.position, transform.rotation);
    }

    // ========== COOLDOWN UTILITY ==========
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
