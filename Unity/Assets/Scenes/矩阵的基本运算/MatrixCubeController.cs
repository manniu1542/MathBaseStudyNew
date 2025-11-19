using UnityEngine;
using UnityEditor;
using System;

public class MatrixCubeController : EditorWindow
{
    private GameObject targetCube;
    private Vector3 matrixPosition = Vector3.zero;
    private Vector3 matrixRotation = Vector3.zero;
    private Vector3 matrixScale = Vector3.one;
    
    // 矩阵显示
    private Matrix4x4 worldMatrix = Matrix4x4.identity;
    private bool showLocalMatrix = true;
    
    // 对比模式
    private bool compareWithTransform = true;
    private Vector3 transformPosition = Vector3.zero;
    private Vector3 transformRotation = Vector3.zero;
    private Vector3 transformScale = Vector3.one;

    [MenuItem("Tools/矩阵Cube控制器")]
    public static void ShowWindow()
    {
        var window = GetWindow<MatrixCubeController>();
        window.titleContent = new GUIContent("矩阵Cube控制器");
        window.minSize = new Vector2(400, 600);
    }

    void OnGUI()
    {
        DrawCubeSelection();
        DrawMatrixControls();
        DrawComparison();
        DrawMatrixInfo();
        
        ApplyMatrixToCube();
    }

    void DrawCubeSelection()
    {
        EditorGUILayout.LabelField("Cube选择", EditorStyles.boldLabel);
        targetCube = (GameObject)EditorGUILayout.ObjectField("目标Cube", targetCube, typeof(GameObject), true);
        
        if (targetCube == null)
        {
            if (GUILayout.Button("创建测试Cube"))
            {
                CreateTestCube();
            }
            return;
        }
        
        EditorGUILayout.Space();
    }

    void DrawMatrixControls()
    {
        EditorGUILayout.LabelField("矩阵变换控制", EditorStyles.boldLabel);
        
        // 位置控制
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            EditorGUILayout.LabelField("平移矩阵", EditorStyles.miniBoldLabel);
            Vector3 newPos = EditorGUILayout.Vector3Field("位置", matrixPosition);
            if (newPos != matrixPosition)
            {
                matrixPosition = newPos;
                UpdateWorldMatrix();
            }
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("世界原点")) matrixPosition = Vector3.zero;
            if (GUILayout.Button("X+5")) matrixPosition.x += 5;
            if (GUILayout.Button("Y+5")) matrixPosition.y += 5;
            if (GUILayout.Button("Z+5")) matrixPosition.z += 5;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        // 旋转控制
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            EditorGUILayout.LabelField("旋转矩阵", EditorStyles.miniBoldLabel);
            Vector3 newRot = EditorGUILayout.Vector3Field("欧拉角", matrixRotation);
            if (newRot != matrixRotation)
            {
                matrixRotation = newRot;
                UpdateWorldMatrix();
            }
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("绕X+30°")) matrixRotation.x += 30;
            if (GUILayout.Button("绕Y+30°")) matrixRotation.y += 30;
            if (GUILayout.Button("绕Z+30°")) matrixRotation.z += 30;
            if (GUILayout.Button("重置旋转")) matrixRotation = Vector3.zero;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        // 缩放控制
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            EditorGUILayout.LabelField("缩放矩阵", EditorStyles.miniBoldLabel);
            Vector3 newScale = EditorGUILayout.Vector3Field("缩放", matrixScale);
            if (newScale != matrixScale)
            {
                matrixScale = newScale;
                UpdateWorldMatrix();
            }
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("等比例2倍")) matrixScale *= 2f;
            if (GUILayout.Button("等比例0.5倍")) matrixScale *= 0.5f;
            if (GUILayout.Button("重置缩放")) matrixScale = Vector3.one;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
    }

    void DrawComparison()
    {
        compareWithTransform = EditorGUILayout.Toggle("与Transform对比", compareWithTransform);
        
        if (compareWithTransform && targetCube != null)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Transform组件值", EditorStyles.miniBoldLabel);
                
                transformPosition = targetCube.transform.position;
                transformRotation = targetCube.transform.eulerAngles;
                transformScale = targetCube.transform.lossyScale;
                
                EditorGUILayout.Vector3Field("Position", transformPosition);
                EditorGUILayout.Vector3Field("Rotation", transformRotation);
                EditorGUILayout.Vector3Field("Scale", transformScale);
                
                // 显示差异
                Vector3 posDiff = matrixPosition - transformPosition;
                Vector3 rotDiff = matrixRotation - transformRotation;
                Vector3 scaleDiff = matrixScale - transformScale;
                
                EditorGUILayout.LabelField("矩阵 vs Transform 差异:");
                EditorGUILayout.Vector3Field("位置差异", posDiff);
                EditorGUILayout.Vector3Field("旋转差异", rotDiff);
                EditorGUILayout.Vector3Field("缩放差异", scaleDiff);
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("从Transform同步到矩阵"))
            {
                SyncFromTransform();
            }
            if (GUILayout.Button("从矩阵应用到Transform"))
            {
                ApplyToTransform();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.Space();
    }

    void DrawMatrixInfo()
    {
        showLocalMatrix = EditorGUILayout.Foldout(showLocalMatrix, "矩阵详细信息", true);
        if (showLocalMatrix)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("世界变换矩阵:", EditorStyles.miniBoldLabel);
                
                for (int i = 0; i < 4; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < 4; j++)
                    {
                        EditorGUILayout.LabelField(worldMatrix[i, j].ToString("F3"), GUILayout.Width(60));
                    }
                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"行列式: {worldMatrix.determinant:F6}");
                EditorGUILayout.LabelField($"是否正交: {IsOrthogonal(worldMatrix)}");
                
                // 提取位置、旋转、缩放
                Vector3 extractedPos = worldMatrix.GetPosition();
                Quaternion extractedRot = worldMatrix.rotation;
                Vector3 extractedScale = worldMatrix.lossyScale;
                
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("从矩阵提取的值:", EditorStyles.miniBoldLabel);
                EditorGUILayout.Vector3Field("位置", extractedPos);
                EditorGUILayout.Vector3Field("旋转", extractedRot.eulerAngles);
                EditorGUILayout.Vector3Field("缩放", extractedScale);
            }
            EditorGUILayout.EndVertical();
        }
    }

    void UpdateWorldMatrix()
    {
        // 构建TRS矩阵（Unity顺序：平移 × 旋转 × 缩放）
        Matrix4x4 translationMatrix = Matrix4x4.Translate(matrixPosition);
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(matrixRotation));
        Matrix4x4 scaleMatrix = Matrix4x4.Scale(matrixScale);
        
        worldMatrix = translationMatrix * rotationMatrix * scaleMatrix;
    }

    void ApplyMatrixToCube()
    {
        if (targetCube == null) return;
        
        // 方法1：直接设置世界矩阵（需要处理层级关系）
        // 方法2：通过Transform组件逐项设置（更直观）
        if (!compareWithTransform)
        {
            // 直接应用矩阵变换（模拟效果）
            UpdateCubeTransform();
        }
    }

    void UpdateCubeTransform()
    {
        // 创建一个临时对象来测试矩阵变换
        GameObject testObj = GameObject.Find("MatrixTestObject");
        if (testObj == null)
        {
            testObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObj.name = "MatrixTestObject";
            testObj.GetComponent<Renderer>().material.color = Color.red;
        }
        
        // 应用矩阵变换
        testObj.transform.position = matrixPosition;
        testObj.transform.rotation = Quaternion.Euler(matrixRotation);
        testObj.transform.localScale = matrixScale;
    }

    void SyncFromTransform()
    {
        if (targetCube == null) return;
        
        matrixPosition = targetCube.transform.position;
        matrixRotation = targetCube.transform.eulerAngles;
        matrixScale = targetCube.transform.lossyScale;
        
        UpdateWorldMatrix();
        Repaint();
    }

    void ApplyToTransform()
    {
        if (targetCube == null) return;
        
        targetCube.transform.position = matrixPosition;
        targetCube.transform.rotation = Quaternion.Euler(matrixRotation);
        targetCube.transform.localScale = matrixScale;
    }

    void CreateTestCube()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "MatrixTestCube";
        cube.transform.position = new Vector3(0, 0, 0);
        
        // 添加一些组件以便观察
        Rigidbody rb = cube.AddComponent<Rigidbody>();
        rb.useGravity = false;
        
        targetCube = cube;
        Selection.activeGameObject = cube;
    }

    bool IsOrthogonal(Matrix4x4 matrix)
    {
        // 检查矩阵是否正交（旋转矩阵的特性）
        Matrix4x4 product = matrix * matrix.transpose;
        return Mathf.Abs(product.m00 - 1f) < 0.001f &&
               Mathf.Abs(product.m11 - 1f) < 0.001f &&
               Mathf.Abs(product.m22 - 1f) < 0.001f;
    }

    // 在Scene视图中绘制辅助信息
    void OnSceneGUI()
    {
        if (targetCube == null) return;
        
        // 绘制坐标轴
        DrawAxes(targetCube.transform);
        
        // 绘制矩阵变换的几何意义
        DrawMatrixVisualization();
    }

    void DrawAxes(Transform transform)
    {
        Handles.color = Color.red;
        Handles.ArrowHandleCap(0, transform.position, transform.rotation * Quaternion.LookRotation(Vector3.right), 1f, EventType.Repaint);
        
        Handles.color = Color.green;
        Handles.ArrowHandleCap(0, transform.position, transform.rotation * Quaternion.LookRotation(Vector3.up), 1f, EventType.Repaint);
        
        Handles.color = Color.blue;
        Handles.ArrowHandleCap(0, transform.position, transform.rotation * Quaternion.LookRotation(Vector3.forward), 1f, EventType.Repaint);
    }

    void DrawMatrixVisualization()
    {
        // 绘制变换过程
        Vector3 origin = Vector3.zero;
        
        // 1. 原始位置
        Handles.color = Color.white;
        Handles.SphereHandleCap(0, origin, Quaternion.identity, 0.1f, EventType.Repaint);
        
        // 2. 应用缩放后的位置
        Vector3 afterScale = Vector3.Scale(origin, matrixScale);
        Handles.color = Color.yellow;
        Handles.DrawDottedLine(origin, afterScale, 5f);
        
        // 3. 应用旋转后的位置
        Vector3 afterRotation = Quaternion.Euler(matrixRotation) * afterScale;
        Handles.color = Color.cyan;
        Handles.DrawDottedLine(afterScale, afterRotation, 5f);
        
        // 4. 最终位置（应用平移）
        Vector3 finalPosition = afterRotation + matrixPosition;
        Handles.color = Color.green;
        Handles.DrawDottedLine(afterRotation, finalPosition, 5f);
        Handles.SphereHandleCap(0, finalPosition, Quaternion.identity, 0.2f, EventType.Repaint);
    }
}