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
    public static void PlayerMovement(bool[] _inputsBool, Vector2[] _inputsVector2)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputsBool.Length);
            foreach (bool _input in _inputsBool)
            {
                _packet.Write(_input);
            }
            _packet.Write(_inputsVector2.Length);
            foreach (Vector2 _input in _inputsVector2)
            {
                _packet.Write(_input);
            }
            //_packet.Write(GameManager.players[Client.instance.myId].transform.rotation);
            // Orientation needs to be the first child of player
            _packet.Write(GameManager.players[Client.instance.myId].transform.GetChild(0).transform.localRotation);
            Quaternion testing = GameManager.players[Client.instance.myId].transform.GetChild(0).transform.localRotation;

            SendUDPData(_packet);
        }
    }

    public static void PlayerShoot(Vector3 _facing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerShoot))
        {
            _packet.Write(_facing);

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