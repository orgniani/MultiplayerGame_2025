using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Managers
{
    public class RacePositionManager : NetworkBehaviour
    {
        private Transform _finishLine;

        [Networked, Capacity(4)]
        private NetworkArray<PlayerRef> _playerOrder => default;

        private readonly Dictionary<PlayerRef, Transform> _playerTransforms = new();

        public void SetFinishLine(Transform finishLine)
        {
            _finishLine = finishLine;
        }

        public void RegisterPlayer(PlayerRef player, Transform playerTransform)
        {
            if (!_playerTransforms.ContainsKey(player))
                _playerTransforms.Add(player, playerTransform);
        }

        public void UnregisterPlayer(PlayerRef player)
        {
            if (_playerTransforms.ContainsKey(player))
                _playerTransforms.Remove(player);
        }

        public List<PlayerRef> GetCurrentPlayerOrder()
        {
            var players = new List<PlayerRef>();
            for (int i = 0; i < _playerOrder.Length; i++)
            {
                var player = _playerOrder.Get(i);
                if (player != default)
                    players.Add(player);
            }
            return players;
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority) return;

            UpdatePlayerPositions();
        }

        private void UpdatePlayerPositions()
        {
            var playerDistances = new List<(PlayerRef player, float distance)>();

            foreach (var kvp in _playerTransforms)
            {
                if (kvp.Value == null) continue;
                float distanceToFinish = Vector3.Distance(kvp.Value.position, _finishLine.position);
                playerDistances.Add((kvp.Key, distanceToFinish));
            }

            playerDistances.Sort((a, b) => a.distance.CompareTo(b.distance));

            for (int i = 0; i < playerDistances.Count; i++)
                _playerOrder.Set(i, playerDistances[i].player);
        }
    }
}
