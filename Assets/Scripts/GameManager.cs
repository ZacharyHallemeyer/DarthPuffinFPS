using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject planetPrefab;
    public GameObject gravityParticleFieldPrefab;
    public GameObject boundaryParticleFieldPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    /// <summary>
    /// Spawns new player to client server and inits player
    /// </summary>
    /// <param name="_id"> client ID </param>
    /// <param name="_username"> client username </param>
    /// <param name="_position"> player spawn position </param>
    /// <param name="_rotation"> player spawn rotation </param>
    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if (_id == Client.instance.myId)
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
        else
            _player = Instantiate(playerPrefab, _position, _rotation);

        _player.GetComponent<PlayerManager>().Initialize(_id, _username);
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

    /// <summary>
    /// Instantiates a new planet at _position with the given scale 
    /// </summary>
    /// <param name="_position"> Origin of new planet </param>
    /// <param name="_localScale"> radius of new planet in Vector3 format </param>
    public void CreatePlanet(Vector3 _position, Vector3 _localScale)
    {
        Color _color = RandomColor();

        GameObject _planet = Instantiate(planetPrefab, _position, Quaternion.identity);
        _planet.transform.localScale = _localScale;

        _planet.GetComponent<MeshRenderer>().material.color = _color;

        // Gravity Particle System
        GameObject _gravityParticleSystem = Instantiate(gravityParticleFieldPrefab, _position, Quaternion.identity);
        _gravityParticleSystem.transform.parent = _planet.transform;

        // Fix local scale
        _gravityParticleSystem.transform.localScale = Vector3.one;

        // Adjust radius
        ParticleSystem.ShapeModule _shapeModule = _gravityParticleSystem.GetComponent<ParticleSystem>().shape;
        // 50 is gravity distance 
        // TODO: add parem of gravity distance
        _shapeModule.radius = _localScale.x + 50 / 2;

        // Random color
        _gravityParticleSystem.GetComponent<ParticleSystemRenderer>().material.color = _color;
        _gravityParticleSystem.GetComponent<ParticleSystemRenderer>().material.SetColor("_EmissionColor", _color);
    }

    /// <summary>
    /// Creates a visual particle system in the shape of a sphere to show the bounds of the play arena
    /// </summary>
    /// <param name="_position"> Origin of particle field sphere </param>
    /// <param name="_radius"> radius of particle field sphere </param>
    public void CreateBoundaryVisual(Vector3 _position, float _radius)
    {
        // Gravity Particle System
        GameObject _gravityParticleSystem = Instantiate(boundaryParticleFieldPrefab, _position, Quaternion.identity);

        // Fix local scale
        _gravityParticleSystem.transform.localScale = Vector3.one;

        // Adjust radius
        ParticleSystem.ShapeModule _shapeModule = _gravityParticleSystem.GetComponent<ParticleSystem>().shape;
        _shapeModule.radius = _radius;

        // Random color
        Color _color = RandomColor();
        _gravityParticleSystem.GetComponent<ParticleSystemRenderer>().material.color = _color;
        _gravityParticleSystem.GetComponent<ParticleSystemRenderer>().material.SetColor("_EmissionColor", _color);
    }

    public Color RandomColor()
    {
        return new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
    }
}
