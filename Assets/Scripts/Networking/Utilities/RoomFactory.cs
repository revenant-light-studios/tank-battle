
using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Realtime;
using TankBattle.Players;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Networking.Utilities
{
    public class RoomFactory
    {
        private static RoomFactory roomFactoryInstance;

        private readonly int _roomNameLength = 7;
        private readonly byte _maxNumberOfPlayers = 8;
        private readonly char[] _roomNameValidChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public static RoomFactory Instance
        {
            get
            {
                if (roomFactoryInstance == null)
                {
                    roomFactoryInstance = new RoomFactory();
                }
                
                return roomFactoryInstance;
            }
        }

        private RoomFactory() {}

        public RoomOptions CreateRoomProperties(string key, bool isVisible)
        {
            int randomSeed = Guid.NewGuid().GetHashCode();
            // Debug.Log($"Random seed generated: {randomSeed}");
            
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = _maxNumberOfPlayers;
            roomOptions.IsVisible = isVisible;
            roomOptions.PublishUserId = true;
            roomOptions.CustomRoomProperties = new Hashtable()
            {
                { RoomOptionsKeys.Name, $"Sala {key}" },
                { RoomOptionsKeys.Seed, randomSeed },
                { RoomOptionsKeys.DefaultPlayerNames, CreateListOfNames()}
            };
            roomOptions.CustomRoomPropertiesForLobby = new string[]
            {
                RoomOptionsKeys.Name,
                RoomOptionsKeys.DefaultPlayerNames
            };
            return roomOptions;
        }

        public string GenerateRoomKey()
        {
            string name = "";
            
            for (int i = 0; i < _roomNameLength; i++)
            {
                name += _roomNameValidChars[Random.Range(0, _roomNameValidChars.Length)];
            }

            return name;
        }

        private string[] CreateListOfNames()
        {
            List<string> _defaultNames;
            PlayerDefaultNames _maleNames = Resources.Load<PlayerDefaultNames>("Settings/DefaultMaleNames");
            PlayerDefaultNames _femaleNames = Resources.Load<PlayerDefaultNames>("Settings/DefaultFemaleNames");
            _defaultNames = new List<string>(_maleNames.Names);
            _defaultNames.AddRange(_femaleNames.Names);
            _defaultNames = _defaultNames.OrderBy(a => Guid.NewGuid()).ToList();
            return _defaultNames.ToArray();
        }
    }
}