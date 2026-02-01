using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponry : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootingPoint;

    [Header("Weapon Settings")]
    [Header("Gun")]
    [SerializeField] private float timeBetweenShots;

    [Header("Lance")]
    [SerializeField] private List<float> chargeTimings;

    [Tooltip("Distance to dash in Unity units (approx meters) per charge level.")]
    [SerializeField] private List<float> dashDistances;

    [Header("Lance Feel")]
    [SerializeField] private float dashDuration = 0.2f;
    [Tooltip("Shape of the dash movement. Steep start = explosive jolt.")]
    [SerializeField] private AnimationCurve dashCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.2f, 0.8f), new Keyframe(1, 1));

    private float _lanceHoldTime;
    private bool _isChargingLance;
    private bool _isDashing; 
    private Rigidbody2D _rb;

    private Coroutine _currentFireCoroutine;
    private Coroutine _dashCoroutine;

    public void Init(Rigidbody2D rb)
    {
        _rb = rb;
    }

    public void Runtime(CharacterInput characterInput)
    {
        if (_isDashing) return;

        HandleGun(characterInput);
        HandleLance(characterInput);
    }

    private void HandleGun(CharacterInput characterInput)
    {
        bool wantsToShoot = characterInput.Shoot && !characterInput.Lance;

        if (wantsToShoot && _currentFireCoroutine == null)
        {
            _currentFireCoroutine = StartCoroutine(ContinuousFire());
        }
        else if (!wantsToShoot && _currentFireCoroutine != null)
        {
            StopCoroutine(_currentFireCoroutine);
            _currentFireCoroutine = null;
        }
    }

    private void HandleLance(CharacterInput characterInput)
    {
        if (characterInput.Lance)
        {
            _isChargingLance = true;
            _lanceHoldTime += Time.deltaTime;

        }
        else if (_isChargingLance)
        {
            float holdTime = _lanceHoldTime;

            _lanceHoldTime = 0f;
            _isChargingLance = false;

            TriggerLanceDash(holdTime);
        }
    }

    private void TriggerLanceDash(float holdTime)
    {
        if (_rb == null) return;
        if (_dashCoroutine != null) StopCoroutine(_dashCoroutine);

        int chargeLevel = 0;
        for (int i = 0; i < chargeTimings.Count; i++)
        {
            if (holdTime >= chargeTimings[i])
            {
                chargeLevel = i + 1;
            }
        }

        chargeLevel = Mathf.Clamp(chargeLevel, 0, dashDistances.Count - 1);
        float distance = dashDistances[chargeLevel];

        _dashCoroutine = StartCoroutine(DashRoutine(distance));
    }

    private IEnumerator DashRoutine(float distance)
    {
        _isDashing = true;

        Vector2 startPos = _rb.position;

        Vector2 direction = transform.up;
        Vector2 targetPos = startPos + (direction * distance);

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            elapsed += Time.fixedDeltaTime;
            float t = elapsed / dashDuration;
            float curveValue = dashCurve.Evaluate(t);

            Vector2 newPos = Vector2.Lerp(startPos, targetPos, curveValue);

            _rb.MovePosition(newPos);

            yield return new WaitForFixedUpdate();
        }

        _rb.MovePosition(Vector2.Lerp(startPos, targetPos, 1f));

        _rb.linearVelocity = Vector2.zero;

        _isDashing = false;
        _dashCoroutine = null;
    }

    IEnumerator ContinuousFire()
    {
        while (true)
        {
            FireBullet();
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    private void FireBullet()
    {
        Quaternion offset = Quaternion.Euler(0, 0, 90f);
        Instantiate(bulletPrefab, shootingPoint.position, transform.rotation * offset);
    }
}