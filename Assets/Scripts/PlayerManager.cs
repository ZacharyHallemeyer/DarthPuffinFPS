using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public float health;
    public float maxHealth;
    public int itemCount = 0;
    public MeshRenderer model;
    public Camera playerCam;
    
    // Grapple
    public LineRenderer lineRenderer;
    private Vector3 grapplePoint;
    public bool isGrappling = false;

    // Testing
    public Transform orientation;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
    }

    private void Update()
    {
        if(!isGrappling)
        {
            if (Input.GetKeyDown(KeyCode.Mouse2))
                ClientSend.PlayerStartGrapple(playerCam.transform.forward);
        }
        if(isGrappling)
        {
            if (Input.GetKeyUp(KeyCode.Mouse2))
            {
                ClientSend.PlayerStopGrapple();
                StopGrapple();
            }
        }

        if (isGrappling)
            DrawRope();
        //TODO: ADD LAYER MASK
        if (Physics.OverlapSphere(transform.position, 10, LayerMask.GetMask("GravityObject")).Length != 0)
        {
            RotatePlayerAccordingToGravity(Physics.OverlapSphere(transform.position, 10, LayerMask.GetMask("GravityObject"))[0]);
        }
    }

    private void FixedUpdate()
    {
        SendInputToServer();
    }

    public void SetHealth(float _health)
    {
        health = _health;

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        model.enabled = false;
    }

    public void Respawn()
    {
        model.enabled = true;
        SetHealth(maxHealth);
    }

    private void SendInputToServer()
    {
        bool[] _inputsBools = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space),
            Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Mouse4),
        };

        Vector2[] _inputVector2 = new Vector2[]
        {
            new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"))
        };

        ClientSend.PlayerMovement(_inputsBools, _inputVector2);
    }

    public void StartGrapple()
    {
        isGrappling = true;
        if (Physics.Raycast(transform.position, playerCam.transform.forward, out RaycastHit _hit))
            grapplePoint = _hit.point;
        lineRenderer.positionCount = 2;
    }

    public void DrawRope()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }

    public void StopGrapple()
    {
        isGrappling = false;
        lineRenderer.positionCount = 0;
    }

    public void RotatePlayerAccordingToGravity(Collider _gravityObjectCollider)
    {
        Debug.Log("Rotate is called");
        Transform _gravityObject = _gravityObjectCollider.transform;
        Quaternion desiredRotation = Quaternion.FromToRotation(_gravityObject.up, -(_gravityObject.position - transform.position).normalized);
        desiredRotation = Quaternion.Lerp(transform.localRotation, desiredRotation, Time.deltaTime * 2);
        transform.localRotation = desiredRotation;
    }

    public Transform[] FindGravityObjects()
    {
        int index = 0;

        Collider[] _gravityObjectColiiders = Physics.OverlapSphere(transform.position, 10, LayerMask.GetMask("GravityObject"));
        Transform[] _gravityObjects = new Transform[_gravityObjectColiiders.Length];
        foreach (Collider _gravityObjectCollider in _gravityObjectColiiders)
        {
            _gravityObjects[index] = _gravityObjectCollider.transform;
            index++;
        }

        return _gravityObjects;
    }
}
