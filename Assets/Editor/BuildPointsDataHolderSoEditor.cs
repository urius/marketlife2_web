using Data;
using Holders;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(BuildPointsDataHolderSo))]
    public class BuildPointsDataHolderSoEditor : UnityEditor.Editor
    {
        private BuildPointsDataHolderSo _targetSo;
        private Grid _grid;

        private void OnEnable()
        {
            _targetSo = (BuildPointsDataHolderSo)serializedObject.targetObject;
            _grid = FindObjectOfType<Grid>();
            
            SceneView.duringSceneGui += DrawGizmos;
        }
 
        private void OnDisable()
        {
            SceneView.duringSceneGui -= DrawGizmos;
        }

        private void DrawGizmos(SceneView sceneView)
        {
            var shopObjectOffset = Constants.ShopObjectRelativeToBuildPointOffset;
            for (var i = 0; i < 15; i++)
            {
                var buildPointData = _targetSo.GetCashDeskBuildPointData(i);

                DrawGizmoDisc(buildPointData.CellCoords);
                DrawGizmoSolidDisc(buildPointData.CellCoords + shopObjectOffset);
                DrawGizmoSolidDisc(buildPointData.CellCoords + shopObjectOffset + Vector2Int.down);

                var truckPointCoords = _targetSo.GetTruckPointCoordsByIndex(i);
                DrawGizmoDisc(truckPointCoords);
                DrawGizmoSolidDisc(truckPointCoords + shopObjectOffset);
                DrawGizmoSolidDisc(truckPointCoords + shopObjectOffset + Vector2Int.down);

                for (var row = 0; row < 20; row++)
                {
                    if (_targetSo.TryGetShelfBuildPointData(row, i, out buildPointData))
                    {
                        DrawGizmoDisc(buildPointData.CellCoords);
                        DrawGizmoSolidDisc(buildPointData.CellCoords + shopObjectOffset);
                        DrawGizmoSolidDisc(buildPointData.CellCoords + shopObjectOffset + Vector2Int.left);
                    }
                }
            }
        }

        private void DrawGizmoDisc(Vector2Int cellCoords)
        {
            var gizmoPosition =
                _grid.GetCellCenterWorld(new Vector3Int(cellCoords.x, cellCoords.y, 0));
            
            Handles.DrawWireDisc(gizmoPosition, Vector3.forward, 0.5f);
        }
        private void DrawGizmoSolidDisc(Vector2Int cellCoords)
        {
            var gizmoPosition =
                _grid.GetCellCenterWorld(new Vector3Int(cellCoords.x, cellCoords.y, 0));
            
            Handles.DrawSolidDisc(gizmoPosition, Vector3.forward, 0.5f);
        }
    }
}