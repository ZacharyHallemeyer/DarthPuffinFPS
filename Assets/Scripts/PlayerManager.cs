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

    // GunObjects
    public class GunInformation
    {
        public string name;
        public float originalGunRadius;
        public GameObject gunContainer;
        public ParticleSystem bullet;
        public ParticleSystem gun;
        public float reloadTime;
        public float fireRate;
    }
    public Dictionary<string, GunInformation> allGunInformation { get; private set; } = new Dictionary<string, GunInformation>();

    public GameObject gunPistol;
    public GameObject gunSMG;
    public GameObject gunAR;
    public GameObject gunShotgun;
    public ParticleSystem pistolMuzzleFlash;
    public ParticleSystem smgMuzzleFlash;
    public ParticleSystem arMuzzleFlash;
    public ParticleSystem shotgunMuzzleFlash;
    public ParticleSystem pistol;
    public ParticleSystem smg;
    public ParticleSystem ar;
    public ParticleSystem shotgun;

    public GameObject shotHitParitcleGameObject;
    public ParticleSystem shotHitParticle;
    public GameObject[] shotgunHitParticleGameObjects;
    public ParticleSystem[] shotgunHitParticles;

    public GunInformation currentGun, secondaryGun;
    private ParticleSystem.ShapeModule shapeModule;
    public GameObject[] gunObjects;
    public bool isAnimInProgress;

    // Testing
    //public string currentGunString;

    #region Set Up

    public void Initialize(int _id, string _username, string _gunName)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        //currentGunString = _gunName;
        SetGunInformation();
        PlayerInitGun(_gunName);
        if (gameObject.name != "LocalPlayer(Clone)")
            enabled = false;
    }

    public void SetGunInformation()
    {
        allGunInformation["Pistol"] = new GunInformation
        {
            name = "Pistol",
            gunContainer = gunPistol,
            bullet = pistolMuzzleFlash,
            gun = pistol,
            originalGunRadius = pistol.shape.radius,
            reloadTime = 1f,
            fireRate = .7f,
        };

        allGunInformation["SMG"] = new GunInformation
        {
            name = "SMG",
            gunContainer = gunSMG,
            bullet = smgMuzzleFlash,
            gun = smg,
            originalGunRadius = smg.shape.radius,
            reloadTime = 1f,
            fireRate = .1f,
        };

        allGunInformation["AR"] = new GunInformation
        {
            name = "AR",
            gunContainer = gunAR,
            bullet = arMuzzleFlash,
            gun = ar,
            originalGunRadius = ar.shape.radius,
            reloadTime = 1f,
            fireRate = .2f,
        };

        allGunInformation["Shotgun"] = new GunInformation
        {
            name = "Shotgun",
            gunContainer = gunShotgun,
            bullet = shotgunMuzzleFlash,
            gun = shotgun,
            originalGunRadius = shotgun.shape.radius,
            reloadTime = 1f,
            fireRate = 1f,
        };
    }

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            ClientSend.PlayerStartShoot(playerCam.transform.position, playerCam.transform.forward);
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            ClientSend.PlayerStopShoot();
        }

        if (!isGrappling)
        {
            if (Input.GetKeyDown(KeyCode.Mouse2))
                ClientSend.PlayerStartGrapple(playerCam.transform.forward);
        }
        if (isGrappling)
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
            RotatePlayerAccordingToGravity(Physics.OverlapSphere(transform.position, 10, LayerMask.GetMask("GravityObject"))[0]);
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
            Input.GetKey(KeyCode.Alpha1),
        };

        ClientSend.PlayerMovement(_inputsBools, isAnimInProgress);
    }

    #region Grapple

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

    #endregion 

    public void RotatePlayerAccordingToGravity(Collider _gravityObjectCollider)
    {
        Transform _gravityObject = _gravityObjectCollider.transform;
        Quaternion desiredRotation = Quaternion.FromToRotation(_gravityObject.up, -(_gravityObject.position - transform.position).normalized);
        desiredRotation = Quaternion.Lerp(transform.localRotation, desiredRotation, Time.deltaTime * 2);
        transform.localRotation = desiredRotation;
    }

    #region Guns

    public void PlayerStartSingleFireAnim()
    {
        isAnimInProgress = true;
        currentGun.gun.Stop();
        currentGun.bullet.Play();
        InvokeRepeating("PlayerSingleFireAnim", currentGun.fireRate, 0f);
    }

    public void PlayerSingleFireAnim()
    {
        Debug.Log("Player single fire is called");
        isAnimInProgress = false;
        currentGun.gun.Play();
    }

    public void PlayerStartAutomaticFireAnim()
    {
        ParticleSystem.RotationOverLifetimeModule rot = currentGun.gun.rotationOverLifetime;
        rot.enabled = true;
    }

    public void PlayerStopAutomaticFireAnim()
    {
        ParticleSystem.RotationOverLifetimeModule rot = currentGun.gun.rotationOverLifetime;
        rot.enabled = false;
    }

    public void PlayerStartReloadAnim()
    {
        isAnimInProgress = true;
        InvokeRepeating("ReloadAnimCompress", 0f, currentGun.reloadTime / 6f);
    }

    public void ReloadAnimCompress()
    {
        if (shapeModule.radius > currentGun.originalGunRadius * 10)
        {
            InvokeRepeating("ReloadAnimExpand", 0f, currentGun.reloadTime / 4);
            CancelInvoke("ReloadAnimCompress");
            return;
        }
        currentGun.gun.Stop();
        shapeModule.radius *= 1.5f;
        currentGun.gun.Play();
    }

    public void ReloadAnimExpand()
    {
        if (shapeModule.radius < currentGun.originalGunRadius)
        {
            shapeModule.radius = currentGun.originalGunRadius;
            isAnimInProgress = false;
            CancelInvoke("ReloadAnimExpand");

            // Reset ammo UI
            //playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);
            return;
        }
        currentGun.gun.Stop();
        shapeModule.radius *= .5f;
        currentGun.gun.Play();
    }

    public void PlayerStartSwitchWeaponAnim()
    {

    }

    public void PlayerInitGun(string _gunName)
    {
        foreach (GunInformation _gunInfo in allGunInformation.Values)
        {
            if (_gunName == _gunInfo.name)
                currentGun = _gunInfo;
        }
        currentGun.gunContainer.SetActive(true);
        shapeModule = currentGun.gun.shape;
    }

    public void ShowOtherPlayerActiveWeapon(int _id, string _gunName)
    {
        if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
        {
            _player = GameManager.players[_id];
        }
        else return;

        foreach (GunInformation _gunInfo in _player.allGunInformation.Values)
        {
            if (_player.allGunInformation[_gunInfo.name].gunContainer.activeSelf == true)
                _player.allGunInformation[_gunInfo.name].gunContainer.SetActive(false);
            if (_gunInfo.name == _gunName)
                _player.currentGun = _gunInfo;
        }
        _player.currentGun.gunContainer.SetActive(true);
    }

    #endregion
}
