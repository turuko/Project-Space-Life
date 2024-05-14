using System.Linq;
using Model.Universe;
using UnityEngine;

namespace Battle.SystemBattle
{
    public record SystemBattleTerrainInfo
    {
        public readonly Vector3[] relativePlanetPositions;
        public readonly PlanetSize[] planetSizes;
        public readonly Vector3 relativeStarPosition;
        public readonly StarType starType;

        public SystemBattleTerrainInfo(StarType type, Vector3 starPos, params (Vector3, PlanetSize)[] planetPositions)
        {
            relativeStarPosition = starPos;
            relativePlanetPositions = planetPositions.Select(x => x.Item1).ToArray();
            planetSizes = planetPositions.Select(x => x.Item2).ToArray();
            starType = type;
        }
    };
}