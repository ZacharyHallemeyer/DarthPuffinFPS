using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        string _gunName = _packet.ReadString();
        int _currentAmmo = _packet.ReadInt();
        int _reserveAmmo = _packet.ReadInt();
        float _maxGrappleTime = _packet.ReadFloat();
        float _maxJetPackTime = _packet.ReadFloat();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation, _gunName, _currentAmmo, 
                                         _reserveAmmo, _maxGrappleTime, _maxJetPackTime);
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
        {
            _player.transform.position = _position;
        }
    }

    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();

        if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
        {
            _player.transform.rotation = _rotation;
        }
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }

    public static void OtherPlayerTakenDamage(Packet _packet)
    {
        int _fromId = _packet.ReadInt();
        int _toId = _packet.ReadInt();

        GameManager.players[_toId].OtherPlayerTakenDamage(_fromId);
    }

    public static void PlayerHealth(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _health = _packet.ReadInt();

        GameManager.players[_id].SetHealth(_health);
    }

    public static void PlayerRespawned(Packet _packet)
    {
        int _id = _packet.ReadInt();

        GameManager.players[_id].Respawn();
    }

    public static void CreateNewPlanet(Packet _packet)
    {
        Vector3 _position = _packet.ReadVector3();
        Vector3 _localScale = _packet.ReadVector3();

        GameManager.instance.CreatePlanet(_position, _localScale);
    }

    public static void CreateBoundary(Packet _packet)
    {
        Vector3 _position = _packet.ReadVector3();
        float _radius = _packet.ReadFloat();

        GameManager.instance.CreateBoundaryVisual(_position, _radius);
    }
    public static void PlayerStartGrapple(Packet _packet)
    {
        int _id = _packet.ReadInt();

        GameManager.playersActions[_id].StartGrapple();
    }

    public static void PlayerContinueGrapple(Packet _packet)
    {
        int _id = _packet.ReadInt();
        float _currentGrappleTime = _packet.ReadFloat();

        GameManager.playersActions[_id].ContinueGrapple(_currentGrappleTime);
    }

    public static void OtherPlayerContinueGrapple(Packet _packet)
    {
        int _fromId = _packet.ReadInt();
        int _toId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Vector3 _grapplePoint = _packet.ReadVector3();

        GameManager.playersActions[_toId].DrawOtherPlayerRope(_fromId, _position, _grapplePoint);
    }

    public static void OtherPlayerStopGrapple(Packet _packet)
    {
        int _fromId = _packet.ReadInt();
        int _toId = _packet.ReadInt();

        GameManager.players[_toId].ClearOtherPlayerRope(_fromId);
    }

    public static void PlayerStopGrapple(Packet _packet)
    {
        int _id = _packet.ReadInt();

        GameManager.playersActions[_id].StopGrapple();
    }

    public static void OtherPlayerSwitchedWeapon(Packet _packet)
    {
        int _fromId = _packet.ReadInt();
        int _toId = _packet.ReadInt();
        string _gunName = _packet.ReadString();

        GameManager.playersActions[_toId].ShowOtherPlayerActiveWeapon(_fromId, _gunName);
    }

    public static void PlayerSingleFire(Packet _packet)
    {
        int _fromId = _packet.ReadInt();
        int _currentAmmo = _packet.ReadInt();
        int _reserveAmmo = _packet.ReadInt();
        GameManager.playersActions[_fromId].PlayerStartSingleFireAnim(_currentAmmo, _reserveAmmo);
    }

    public static void PlayerStartAutomaticFire(Packet _packet)
    {
        int _fromId = _packet.ReadInt();
        int _currentAmmo = _packet.ReadInt();
        int _reserveAmmo = _packet.ReadInt();
        GameManager.playersActions[_fromId].PlayerStartAutomaticFireAnim(_currentAmmo, _reserveAmmo);
    }

    public static void PlayerContinueAutomaticFire(Packet _packet)
    {
        int _fromId = _packet.ReadInt();
        int _currentAmmo = _packet.ReadInt();
        int _reserveAmmo = _packet.ReadInt();
        GameManager.playersActions[_fromId].PlayerContinueAutomaticFireAnim(_currentAmmo, _reserveAmmo);
    }   

    public static void PlayerStopAutomaticFire(Packet _packet)
    {
        int _fromId = _packet.ReadInt();
        GameManager.playersActions[_fromId].PlayerStopAutomaticFireAnim();
    }

    public static void PlayerReload(Packet _packet)
    {
        int _fromId = _packet.ReadInt();
        int _currentAmmo = _packet.ReadInt();
        int _reserveAmmo = _packet.ReadInt();
        GameManager.playersActions[_fromId].PlayerStartReloadAnim(_currentAmmo, _reserveAmmo);
    }

    public static void PlayerSwitchWeapon(Packet _packet)
    {
        int _fromId = _packet.ReadInt();
        int _currentAmmo = _packet.ReadInt();
        int _reserveAmmo = _packet.ReadInt();
        string _newGunName = _packet.ReadString();

        GameManager.playersActions[_fromId].PlayerStartSwitchWeaponAnim(_newGunName, _currentAmmo, _reserveAmmo);
    }

    public static void PlayerShotLanded(Packet _packet)
    {
        int _fromId = _packet.ReadInt();
        Vector3 _hitPoint = _packet.ReadVector3();

        GameManager.playersActions[_fromId].PlayerShotLanded(_hitPoint);
    }

    public static void PlayerContinueJetPack(Packet _packet)
    {
        int _fromId = _packet.ReadInt();
        float _jetPackTime = _packet.ReadFloat();

        GameManager.playersMovement[_fromId].PlayerContinueJetPack(_jetPackTime);
    }
}