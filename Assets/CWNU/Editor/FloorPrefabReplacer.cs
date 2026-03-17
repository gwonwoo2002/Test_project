using UnityEngine;
using UnityEditor;

public class FloorPrefabReplacer : EditorWindow
{
    private enum Tab { Replace, Snap }
    private Tab currentTab = Tab.Replace;

    // --- Replace 탭 ---
    private GameObject replacementPrefab;
    private int replaceCount = 0;

    // --- Snap 탭 ---
    private float detectedSize = 0f;
    private float fixedY = 0f;
    private bool useFixedY = true;

    [MenuItem("CWNU Tools/Floor Prefab Replacer")]
    public static void OpenWindow()
    {
        GetWindow<FloorPrefabReplacer>("Floor Prefab Replacer");
    }

    private void OnGUI()
    {
        currentTab = (Tab)GUILayout.Toolbar((int)currentTab, new string[] { "Replace", "Grid Snap" });
        EditorGUILayout.Space();

        if (currentTab == Tab.Replace)
            DrawReplaceTab();
        else
            DrawSnapTab();
    }

    // ────────────────────────────────────────
    // Replace 탭
    // ────────────────────────────────────────
    private void DrawReplaceTab()
    {
        GUILayout.Label("선택한 오브젝트를 다른 프리팹으로 교체", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        int selected = Selection.gameObjects.Length;
        EditorGUILayout.HelpBox(
            selected > 0 ? $"선택된 오브젝트: {selected}개" : "Hierarchy에서 교체할 오브젝트를 선택하세요.",
            selected > 0 ? MessageType.Info : MessageType.Warning);

        EditorGUILayout.Space();

        replacementPrefab = (GameObject)EditorGUILayout.ObjectField(
            "New Prefab", replacementPrefab, typeof(GameObject), false);

        EditorGUILayout.Space();

        GUI.enabled = selected > 0 && replacementPrefab != null;
        if (GUILayout.Button("Replace Selected", GUILayout.Height(35)))
        {
            if (EditorUtility.DisplayDialog("Replace Prefabs",
                $"선택된 {selected}개를 '{replacementPrefab.name}' 으로 교체합니다.\n계속하시겠습니까?",
                "교체", "취소"))
            {
                ReplaceSelected();
            }
        }
        GUI.enabled = true;

        if (replaceCount > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox($"완료: {replaceCount}개 교체됨", MessageType.Info);
        }
    }

    private void ReplaceSelected()
    {
        replaceCount = 0;
        Undo.SetCurrentGroupName("Replace Selected Prefabs");
        int undoGroup = Undo.GetCurrentGroup();

        GameObject[] targets = Selection.gameObjects;

        foreach (GameObject obj in targets)
        {
            Vector3 pos = obj.transform.position;
            Quaternion rot = obj.transform.rotation;
            Vector3 scale = obj.transform.localScale;
            Transform parent = obj.transform.parent;
            int siblingIndex = obj.transform.GetSiblingIndex();

            GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(replacementPrefab);
            Undo.RegisterCreatedObjectUndo(newObj, "Replace Prefab");

            newObj.transform.position = pos;
            newObj.transform.rotation = rot;
            newObj.transform.localScale = scale;
            newObj.transform.SetParent(parent);
            newObj.transform.SetSiblingIndex(siblingIndex);

            Undo.DestroyObjectImmediate(obj);
            replaceCount++;
        }

        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log($"[FloorPrefabReplacer] {replaceCount}개 교체 완료 → {replacementPrefab.name}");
    }

    // ────────────────────────────────────────
    // Grid Snap 탭
    // ────────────────────────────────────────
    private void DrawSnapTab()
    {
        GUILayout.Label("Z-fighting 해결 — 타일 겹침 제거", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        int selected = Selection.gameObjects.Length;
        EditorGUILayout.HelpBox(
            selected > 0 ? $"선택된 오브젝트: {selected}개" : "Hierarchy에서 타일들을 선택하세요.",
            selected > 0 ? MessageType.Info : MessageType.Warning);

        EditorGUILayout.Space();

        // 타일 크기 자동 감지
        GUILayout.Label("Step 1 — 타일 실제 크기 감지", EditorStyles.boldLabel);

        if (GUILayout.Button("Auto Detect Tile Size (선택 오브젝트 기준)"))
            DetectTileSize();

        if (detectedSize > 0f)
            EditorGUILayout.HelpBox($"감지된 타일 크기: {detectedSize}", MessageType.Info);
        else
            EditorGUILayout.HelpBox("버튼을 눌러 타일 크기를 먼저 감지하세요.", MessageType.Warning);

        detectedSize = EditorGUILayout.FloatField("Tile Size (직접 입력 가능)", detectedSize);

        EditorGUILayout.Space();

        // Y 고정
        GUILayout.Label("Step 2 — Y Position 고정", EditorStyles.boldLabel);
        useFixedY = EditorGUILayout.Toggle("Y 고정", useFixedY);
        if (useFixedY)
            fixedY = EditorGUILayout.FloatField("Fixed Y", fixedY);

        EditorGUILayout.Space();

        // 실행
        GUI.enabled = selected > 0 && detectedSize > 0f;
        if (GUILayout.Button("Re-Space Selected Tiles", GUILayout.Height(35)))
        {
            if (EditorUtility.DisplayDialog("Re-Space Tiles",
                $"선택된 {selected}개 타일을 크기({detectedSize}) 기준으로 재배치합니다.\n겹침이 제거됩니다.",
                "실행", "취소"))
            {
                ReSpaceTiles();
            }
        }
        GUI.enabled = true;
    }

    private void DetectTileSize()
    {
        GameObject[] targets = Selection.gameObjects;
        if (targets.Length == 0) return;

        // 첫 번째 오브젝트의 Renderer bounds로 실제 크기 측정
        Renderer r = targets[0].GetComponentInChildren<Renderer>();
        if (r == null)
        {
            Debug.LogWarning("[FloorPrefabReplacer] Renderer를 찾을 수 없습니다.");
            return;
        }

        // X, Z 중 큰 값을 타일 크기로 사용 (정사각형 타일 가정)
        float sizeX = r.bounds.size.x;
        float sizeZ = r.bounds.size.z;
        detectedSize = Mathf.Max(sizeX, sizeZ);

        Debug.Log($"[FloorPrefabReplacer] 감지된 타일 크기: {detectedSize} (bounds X:{sizeX:F4} Z:{sizeZ:F4})");
        Repaint();
    }

    private void ReSpaceTiles()
    {
        Undo.SetCurrentGroupName("Re-Space Floor Tiles");
        int undoGroup = Undo.GetCurrentGroup();

        GameObject[] targets = Selection.gameObjects;
        float tileSize = detectedSize;

        // 선택된 타일 중 가장 작은 X, Z를 앵커로 사용
        float minX = float.MaxValue;
        float minZ = float.MaxValue;
        foreach (GameObject obj in targets)
        {
            if (obj.transform.position.x < minX) minX = obj.transform.position.x;
            if (obj.transform.position.z < minZ) minZ = obj.transform.position.z;
        }

        // 앵커를 tileSize 그리드에 정렬
        minX = Mathf.Round(minX / tileSize) * tileSize;
        minZ = Mathf.Round(minZ / tileSize) * tileSize;

        int count = 0;
        foreach (GameObject obj in targets)
        {
            Undo.RecordObject(obj.transform, "Re-Space");

            // 현재 위치를 기준으로 그리드 인덱스 계산
            int ix = Mathf.RoundToInt((obj.transform.position.x - minX) / tileSize);
            int iz = Mathf.RoundToInt((obj.transform.position.z - minZ) / tileSize);

            Vector3 newPos = obj.transform.position;
            newPos.x = minX + ix * tileSize;
            newPos.z = minZ + iz * tileSize;
            if (useFixedY) newPos.y = fixedY;

            obj.transform.position = newPos;
            count++;
        }

        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log($"[FloorPrefabReplacer] {count}개 타일 재배치 완료 (tileSize={tileSize:F4})");
    }

    private void OnSelectionChange() => Repaint();
}
