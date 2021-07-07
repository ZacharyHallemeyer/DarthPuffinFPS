using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    /// <summary>Sends a packet to the server via TCP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to the server via UDP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(PlayerPrefs.GetString("Username", "Null"));

            SendTCPData(_packet);
        }
    }

    /// <summary>Sends player input to the server.</summary>
    /// <param name="_inputs"></param>
    public static void PlayerMovement(Vector2 _moveDirection)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_moveDirection);
            // Orientation needs to be the first child of player
            _packet.Write(GameManager.players[Client.instance.myId].transform.GetChild(0).transform.localRotation);

            SendUDPData(_packet);
        }
    }

    /// <summary>Sends player input to the server.</summary>
    /// <param name="_inputs"></param>
    public static void PlayerActions(bool _isAnimInProgress)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerActions))
        {
            _packet.Write(_isAnimInProgress);

            SendUDPData(_packet);
        }
    }

    public static void PlayerJetPackMovement(Vector3 _direction)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerJetPackMovement))
        {
            _packet.Write(_direction);

            SendUDPData(_packet);
        }
    }

    public static void PlayerMagnetize()
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMagnetize))
        {
            SendTCPData(_packet);
        }
    }

    public static void PlayerStartGrapple(Vector3 _direction)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerStartGrapple))
        {
            _packet.Write(_direction);

            SendTCPData(_packet);
        }
    }

    public static void PlayerContinueGrappling(Vector3 _position, Vector3 _grapplePoint)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerContinueGrappling))
        {
            _packet.Write(_position);
            _packet.Write(_grapplePoint);

            SendUDPData(_packet);
        }
    }

    public static void PlayerStopGrapple()
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerStopGrapple))
        {
            SendTCPData(_packet);
        }
    }

    public static void PlayerStartShoot(Vector3 _firePoint, Vector3 _fireDirection)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerStartShoot))
        {
            _packet.Write(_firePoint);
            _packet.Write(_fireDirection);

            SendTCPData(_packet);
        }
    }

    public static void PlayerUpdateShootDirection(Vector3 _firePoint, Vector3 _fireDirection)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerUpdateShootDirection))
        {
            _packet.Write(_firePoint);
            _packet.Write(_fireDirection);

            SendUDPData(_packet);
        }
    }

    public static void PlayerStopShoot()
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerStopShoot))
        {
            SendTCPData(_packet);
        }
    }

    public static void PlayerReload()
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerReload))
        {
            SendTCPData(_packet);
        }
    }

    public static void PlayerSwitchWeapon()
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerSwitchWeapon))
        {
            SendTCPData(_packet);
        }
    }

    public static void PlayerThrowItem(Vector3 _facing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerThrowItem))
        {
            _packet.Write(_facing);

            SendTCPData(_packet);
        }
    }

    #endregion
}