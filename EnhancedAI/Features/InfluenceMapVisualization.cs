using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Rendering.UI;
using BattleTech.UI;
using UnityEngine;
using UnityEngine.Rendering;

namespace EnhancedAI.Features
{
    public static class InfluenceMapVisualization
    {
        private static GameObject _parent = new GameObject("InfluenceMapVisualizationParent");
        private static List<GameObject> _unusedDotPool = new List<GameObject>();
        private static List<GameObject> _usedDotPool = new List<GameObject>();
        private static Mesh _circleMesh = GenerateCircleMesh(4, 20);
        private static Vector3 _groundOffset = 2 * Vector3.up;


        public static void Show()
        {
            _parent.SetActive(true);
        }

        public static void OnInfluenceMapSort(AbstractActor unit)
        {
            Hide();

            var map = unit.BehaviorTree.influenceMapEvaluator;
            var hexToPos = new Dictionary<MapTerrainDataCell, Vector3>();
            var hexToMax = new Dictionary<MapTerrainDataCell, float>();
            var highest = float.MinValue;

            for (var i = 0; i < map.firstFreeWorkspaceEvaluationEntryIndex; i++)
            {
                var entry = map.WorkspaceEvaluationEntries[i];
                var value = entry.GetHighestAccumulator();
                var hex = unit.Combat.MapMetaData.GetCellAt(entry.Position);

                hexToPos[hex] = entry.Position;

                // find the highest at hex
                if (hexToMax.ContainsKey(hex))
                    hexToMax[hex] = Mathf.Max(hexToMax[hex], value);
                else
                    hexToMax.Add(hex, value);

                highest = Mathf.Max(highest, value);
            }

            var lowest = hexToMax.Values.Min();
            var average = hexToMax.Values.Average();

            foreach (var kvp in hexToMax)
            {
                var hex = kvp.Key;
                var value = kvp.Value;

                var darkGrey = new Color(.15f, .15f, .15f);
                Color color;
                color = value <= average
                    ? Color.Lerp(Color.red, darkGrey, (value - lowest) / (average - lowest))
                    : Color.Lerp(darkGrey, Color.cyan, (value - average) / (highest - average));

                //color = Color.Lerp(Color.black, Color.white, (value - lowest) / (highest - lowest));

                ShowDotAt(hexToPos[hex], color);
            }
        }

        public static void Hide()
        {
            foreach (var dot in _usedDotPool)
                dot.SetActive(false);

            _unusedDotPool.AddRange(_usedDotPool);
            _usedDotPool.Clear();
            _parent.SetActive(false);
        }


        private static void ShowDotAt(Vector3 location, Color color)
        {
            GameObject dot;
            if (_unusedDotPool.Count > 0)
            {
                dot = _unusedDotPool[0];
                _unusedDotPool.RemoveAt(0);
            }
            else
            {
                var movementDot = CombatMovementReticle.Instance.movementDotTemplate;

                dot = new GameObject($"dot_{_unusedDotPool.Count + _usedDotPool.Count}");
                dot.transform.SetParent(_parent.transform);

                var meshFilter = dot.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = _circleMesh;

                var meshRenderer = dot.AddComponent<MeshRenderer>();
                meshRenderer.material = movementDot.GetComponent<MeshRenderer>().sharedMaterial;
                meshRenderer.material.enableInstancing = false;
                meshRenderer.receiveShadows = false;
                meshRenderer.shadowCastingMode = ShadowCastingMode.Off;

                dot.AddComponent<UISweep>();
            }

            _usedDotPool.Add(dot);
            dot.transform.position = location + _groundOffset;
            dot.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            var renderer = dot.GetComponent<MeshRenderer>();
            renderer.material.color = color;

            dot.SetActive(true);
        }

        private static Mesh GenerateCircleMesh(float size, int numberOfPoints)
        {
            // from https://answers.unity.com/questions/944228/creating-a-smooth-round-flat-circle.html
            // not subject to license
            var angleStep = 360.0f / numberOfPoints;
            var vertexList = new List<Vector3>();
            var triangleList = new List<int>();
            var quaternion = Quaternion.Euler(0.0f, 0.0f, angleStep);

            vertexList.Add(new Vector3(0.0f, 0.0f, 0.0f));
            vertexList.Add(new Vector3(0.0f, size, 0.0f));
            vertexList.Add(quaternion * vertexList[1]);
            triangleList.Add(0);
            triangleList.Add(1);
            triangleList.Add(2);

            for (var i = 0; i < numberOfPoints - 1; i++)
            {
                triangleList.Add(0);
                triangleList.Add(vertexList.Count - 1);
                triangleList.Add(vertexList.Count);
                vertexList.Add(quaternion * vertexList[vertexList.Count - 1]);
            }
            var mesh = new Mesh();
            mesh.vertices = vertexList.ToArray();
            mesh.triangles = triangleList.ToArray();

            return mesh;
        }
    }
}
