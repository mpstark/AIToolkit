using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Rendering.UI;
using BattleTech.UI;
using UnityEngine;
using UnityEngine.Rendering;

// ReSharper disable AccessToStaticMemberViaDerivedType

namespace EnhancedAI.Features.UI
{
    public class InfluenceMapVisualization
    {
        public GameObject ParentObject;

        private List<GameObject> _unusedDotPool = new List<GameObject>();
        private List<GameObject> _usedDotPool = new List<GameObject>();
        private Mesh _circleMesh = GenerateCircleMesh(4, 20);
        private Vector3 _groundOffset = 2 * Vector3.up;

        private TextPopup _tooltip;
        private const string TooltipName = "EnhancedAIInfluenceMapTooltip";


        public InfluenceMapVisualization(string name)
        {
            ParentObject = new GameObject(name);
            ParentObject.SetActive(false);

            var oldTooltip = GameObject.Find(TooltipName);
            if (oldTooltip != null)
                GameObject.Destroy(oldTooltip);

            _tooltip = new TextPopup(TooltipName, true);
        }


        public void Show()
        {
            ParentObject.SetActive(true);
        }

        public void OnInfluenceMapSort(AbstractActor unit)
        {
            Hide();

            var map = unit.BehaviorTree.influenceMapEvaluator;

            var lowest = float.MaxValue;
            var highest = float.MinValue;
            var total = 0f;
            var hexToHighestData = new Dictionary<MapTerrainDataCell, HexFactorData>();

            for (var i = 0; i < map.firstFreeWorkspaceEvaluationEntryIndex; i++)
            {
                var entry = map.WorkspaceEvaluationEntries[i];
                var hex = unit.Combat.MapMetaData.GetCellAt(entry.Position);
                var value = entry.GetHighestAccumulator();

                highest = Mathf.Max(highest, value);
                lowest = Mathf.Min(lowest, value);
                total += value;

                if (hexToHighestData.ContainsKey(hex) && hexToHighestData[hex].Value > value)
                    continue;

                var data = new HexFactorData
                {
                    FactorRecords = entry.ValuesByFactorName,
                    MoveType = entry.GetBestMoveType(),
                    Position = entry.Position,
                    Value = value
                };

                hexToHighestData[hex] = data;
            }

            var average = total / map.firstFreeWorkspaceEvaluationEntryIndex;
            foreach (var hexData in hexToHighestData.Values)
            {
                var darkGrey = new Color(.15f, .15f, .15f);
                var color = hexData.Value <= average
                    ? Color.Lerp(Color.red, darkGrey, (hexData.Value - lowest) / (average - lowest))
                    : Color.Lerp(darkGrey, Color.cyan, (hexData.Value - average) / (highest - average));

                ShowDotAt(hexData.Position, color, hexData);
            }
        }

        public void Hide()
        {
            foreach (var dot in _usedDotPool)
                dot.SetActive(false);

            _unusedDotPool.AddRange(_usedDotPool);
            _usedDotPool.Clear();
            ParentObject.SetActive(false);

            DotTooltip.CompareAgainst = null;
        }

        private void ShowDotAt(Vector3 location, Color color, HexFactorData factorData)
        {
            GameObject dot;
            if (_unusedDotPool.Count > 0)
            {
                dot = _unusedDotPool[0];
                _unusedDotPool.RemoveAt(0);
            }
            else
            {
                dot = GenerateDot($"dot_{_unusedDotPool.Count + _usedDotPool.Count}");
            }

            _usedDotPool.Add(dot);
            dot.transform.position = location + _groundOffset;
            dot.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            var tooltip = dot.GetComponent<DotTooltip>();
            tooltip.FactorData = factorData;

            var renderer = dot.GetComponent<MeshRenderer>();
            renderer.material.color = color;

            dot.SetActive(true);
        }


        private GameObject GenerateDot(string name)
        {
            var dot = new GameObject(name);
            dot.transform.SetParent(ParentObject.transform);

            var meshFilter = dot.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = _circleMesh;

            var meshRenderer = dot.AddComponent<MeshRenderer>();
            var movementDot = CombatMovementReticle.Instance.movementDotTemplate;
            meshRenderer.material = movementDot.GetComponent<MeshRenderer>().sharedMaterial;
            meshRenderer.material.enableInstancing = false;
            meshRenderer.receiveShadows = false;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;

            var collider = dot.AddComponent<CapsuleCollider>();
            collider.center = Vector3.zero;
            collider.radius = 5f;
            collider.height = .5f;
            collider.isTrigger = true;

            var dotTooltip = dot.AddComponent<DotTooltip>();
            dotTooltip.TooltipPopup = _tooltip;

            dot.AddComponent<UISweep>();

            return dot;
        }

        private static Mesh GenerateCircleMesh(float radius, int numberOfPoints)
        {
            // from https://answers.unity.com/questions/944228/creating-a-smooth-round-flat-circle.html
            // not subject to license
            var angleStep = 360.0f / numberOfPoints;
            var vertexList = new List<Vector3>();
            var triangleList = new List<int>();
            var quaternion = Quaternion.Euler(0.0f, 0.0f, angleStep);

            vertexList.Add(new Vector3(0.0f, 0.0f, 0.0f));
            vertexList.Add(new Vector3(0.0f, radius, 0.0f));
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

    public class HexFactorData
    {
        public Vector3 Position;
        public float Value;
        public MoveType MoveType;
        public Dictionary<string, EvaluationDebugLogRecord> FactorRecords;
    }

    public class DotTooltip : MonoBehaviour
    {
        public static HexFactorData CompareAgainst;
        public HexFactorData FactorData;
        public TextPopup TooltipPopup;


        public void OnMouseEnter()
        {
            TooltipPopup.SetText(GetTooltipString());
        }

        public void OnMouseExit()
        {
            TooltipPopup.Hide();
        }

        public void OnMouseDown()
        {
            if (CompareAgainst == FactorData)
                CompareAgainst = null;
            else
                CompareAgainst = FactorData;
        }


        private string GetTooltipString()
        {
            var factors = new Dictionary<string, float>();
            foreach (var recordKVP in FactorData.FactorRecords)
                factors.Add(recordKVP.Key, GetEntryValue(recordKVP.Value, FactorData.MoveType));

            var value = FactorData.Value;
            if (CompareAgainst != null)
            {
                foreach (var recordKVP in CompareAgainst.FactorRecords)
                    factors[recordKVP.Key] -= GetEntryValue(recordKVP.Value, CompareAgainst.MoveType);

                value -= CompareAgainst.Value;
            }
            var tooltip = $"<size=100%><u><b>{value:0.00}</b> [{Enum.GetName(typeof(MoveType), FactorData.MoveType)}]</u><size=80%>";

            if (CompareAgainst != null)
                tooltip += " COMPARISON";

            var sortedNames = factors.Keys.ToList();
            sortedNames.Sort((one, two) =>
                Mathf.Abs(factors[one]).CompareTo(Mathf.Abs(factors[two])));
            sortedNames.Reverse();

            foreach (var factorName in sortedNames)
            {
                if (Math.Abs(factors[factorName]) > 0.01)
                    tooltip += $"\n{factors[factorName]:0.00} : {factorName}";
            }

            return tooltip;
        }

        private static float GetEntryValue(EvaluationDebugLogRecord record, MoveType moveType)
        {
            return moveType == MoveType.Sprinting ? record.SprintValue : record.RegularValue;
        }
    }
}
