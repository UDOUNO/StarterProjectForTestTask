using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;

    [Header("Fall")]
    [SerializeField] private float _fallAngle = 30f;
    [SerializeField] private float _fallDelay = 1f;
    [SerializeField] private float _fallSpeed = 2f;
    [SerializeField] private float _fallRotationSpeed = 180f;

    private EventManager _eventManager;

    private bool _isFalling;
    private float _fallTimer;

    [Inject]
    public void Initialize(EventManager eventManager)
    {
        _eventManager = eventManager;
    }

    private void Update()
    {
        if (_isFalling)
        {
            UpdateFalling();
            return;
        }

        Move();
        CheckGround();
    }

    private void Move()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                input.y += 1f;

            if (Keyboard.current.sKey.isPressed)
                input.y -= 1f;

            if (Keyboard.current.dKey.isPressed)
                input.x += 1f;

            if (Keyboard.current.aKey.isPressed)
                input.x -= 1f;
        }

        Vector3 direction = new Vector3(
            input.x,
            0f,
            input.y
        ).normalized;

        transform.position += direction * _moveSpeed * Time.deltaTime;
    }

    private void CheckGround()
    {
        if (!Physics.Raycast(
                transform.position + Vector3.up * 0.1f,
                Vector3.down,
                out RaycastHit hit,
                2f))
        {
            return;
        }

        float surfaceAngle = Vector3.Angle(
            hit.normal,
            Vector3.up
        );

        if (surfaceAngle >= _fallAngle)
        {
            StartFalling();
        }
    }

    private void StartFalling()
    {
        if (_isFalling)
            return;

        _isFalling = true;
        _fallTimer = 0f;
    }

    private void UpdateFalling()
    {
        _fallTimer += Time.deltaTime;

        transform.Rotate(
            Vector3.right,
            _fallRotationSpeed * Time.deltaTime
        );

        transform.position +=
            Vector3.down * _fallSpeed * Time.deltaTime;

        if (_fallTimer >= _fallDelay)
        {
            PublishFallEvent();
        }
    }

    private void PublishFallEvent()
    {
        _eventManager.Publish(
            new EventsProvider.PlayerFellEvent()
        );
    }
}