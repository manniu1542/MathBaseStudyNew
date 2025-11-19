using UnityEngine;
using UnityEditor;
using System;

public class MatrixTransformSandbox : EditorWindow
{
    // 变换矩阵组件
    private Vector3 translation = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 scale = Vector3.one;
    private float shearX = 0f;
    private float shearY = 0f;

    // 交互控制
    private Rect interactiveRect = new Rect(50, 50, 100, 80);
    private bool isDragging = false;
    private bool isScaling = false;
    private Vector2 dragStartPos;
    private Rect originalRect;

    // 可视化设置
    private bool showDecomposition = true;
    private bool showGrid = true;
    private float animationTime = 0f;
    private bool isAnimating = false;

    // 交互区域定义
    private Rect interactiveAreaRect = new Rect(10, 420, 1920, 300);

    [MenuItem("Tools/矩阵变换沙盒")]
    public static void ShowWindow()
    {
        var window = GetWindow<MatrixTransformSandbox>();
        window.titleContent = new GUIContent("矩阵变换沙盒");
        window.minSize = new Vector2(600, 700);
    }

    void OnGUI()
    {
        DrawToolbar();
        DrawMatrixControls();
        DrawInteractiveArea();
        DrawVisualization();

        HandleMouseEvents();

        if (isAnimating)
        {
            animationTime += 0.02f;
            if (animationTime >= 1f) animationTime = 0f;
            Repaint();
        }
    }

    void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            if (GUILayout.Button("重置", EditorStyles.toolbarButton))
            {
                ResetTransform();
            }

            if (GUILayout.Button(isAnimating ? "停止动画" : "播放动画", EditorStyles.toolbarButton))
            {
                isAnimating = !isAnimating;
            }

            GUILayout.FlexibleSpace();

            showDecomposition = GUILayout.Toggle(showDecomposition, "显示分解", EditorStyles.toolbarButton);
            showGrid = GUILayout.Toggle(showGrid, "显示网格", EditorStyles.toolbarButton);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    void DrawMatrixControls()
    {
        EditorGUILayout.LabelField("矩阵变换参数", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            EditorGUILayout.LabelField("平移矩阵", EditorStyles.miniBoldLabel);
            translation = EditorGUILayout.Vector3Field("位移", translation);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("←")) translation.x -= 10;
            if (GUILayout.Button("→")) translation.x += 10;
            if (GUILayout.Button("↑")) translation.y += 10;
            if (GUILayout.Button("↓")) translation.y -= 10;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            EditorGUILayout.LabelField("旋转矩阵", EditorStyles.miniBoldLabel);
            rotation = EditorGUILayout.Vector3Field("欧拉角", rotation);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("绕Z轴 +15°")) rotation.z += 15;
            if (GUILayout.Button("绕Z轴 -15°")) rotation.z -= 15;
            if (GUILayout.Button("重置旋转")) rotation = Vector3.zero;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            EditorGUILayout.LabelField("缩放矩阵", EditorStyles.miniBoldLabel);
            scale = EditorGUILayout.Vector3Field("缩放", scale);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("放大X")) scale.x *= 1.2f;
            if (GUILayout.Button("缩小X")) scale.x /= 1.2f;
            if (GUILayout.Button("等比例放大")) scale *= 1.2f;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            EditorGUILayout.LabelField("剪切矩阵", EditorStyles.miniBoldLabel);
            shearX = EditorGUILayout.Slider("X方向剪切", shearX, -1f, 1f);
            shearY = EditorGUILayout.Slider("Y方向剪切", shearY, -1f, 1f);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
    }

    void DrawInteractiveArea()
    {
    
        EditorGUILayout.LabelField("交互区域 (拖动矩形或控制点)", EditorStyles.boldLabel);
        
        // 为交互区域预留空间
        GUILayout.Space(interactiveAreaRect.height);

        // 在GUI层级之上绘制
        if (Event.current.type == EventType.Repaint)
        {
            // 绘制背景
            EditorGUI.DrawRect(interactiveAreaRect, new Color(0.15f, 0.15f, 0.15f, 1f));

            if (showGrid)
            {
                DrawGrid(interactiveAreaRect);
            }

            // 获取当前变换矩阵
            Matrix4x4 transformMatrix = GetCurrentTransformMatrix();
            Vector3[] originalCorners = GetRectCorners(interactiveRect);
            Vector3[] transformedCorners = TransformPoints(originalCorners, transformMatrix);

            // 开始GUI绘制
            Handles.BeginGUI();

            // 绘制原始矩形（半透明）
            Handles.color = new Color(1, 1, 1, 0.3f);
            Handles.DrawSolidRectangleWithOutline(interactiveAreaRect, new Color(0,0,0,0), new Color(1,1,1,0.2f));
            
            // 绘制原始矩形边框
            Handles.color = Color.gray;
            for (int i = 0; i < 4; i++)
            {
                int next = (i + 1) % 4;
                Handles.DrawLine(originalCorners[i], originalCorners[next]);
            }

            // 绘制变换后的矩形
            Handles.color = Color.cyan;
            for (int i = 0; i < 4; i++)
            {
                int next = (i + 1) % 4;
                Handles.DrawLine(transformedCorners[i], transformedCorners[next]);
            }

            // 绘制变换后的角点
            for (int i = 0; i < 4; i++)
            {
                Handles.DrawSolidDisc(transformedCorners[i], Vector3.forward, 4f);
            }

            // 绘制分解动画
            if (showDecomposition && isAnimating)
            {
                DrawTransformationDecomposition(originalCorners);
            }

            Handles.EndGUI();
        }
    }

    void DrawVisualization()
    {
        EditorGUILayout.LabelField("矩阵信息", EditorStyles.boldLabel);

        Matrix4x4 currentMatrix = GetCurrentTransformMatrix();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            EditorGUILayout.LabelField("当前变换矩阵:", EditorStyles.miniBoldLabel);

            for (int i = 0; i < 4; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < 4; j++)
                {
                    EditorGUILayout.LabelField(currentMatrix[i, j].ToString("F2"), GUILayout.Width(50));
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            EditorGUILayout.LabelField("变换类型分析:", EditorStyles.miniBoldLabel);

            string transformType = "刚体变换";
            if (scale != Vector3.one) transformType = "仿射变换";
            if (Mathf.Abs(shearX) > 0.01f || Mathf.Abs(shearY) > 0.01f) transformType = "投影变换";

            EditorGUILayout.LabelField($"类型: {transformType}");
            EditorGUILayout.LabelField($"行列式: {currentMatrix.determinant:F4}");
        }
        EditorGUILayout.EndVertical();
    }

    void HandleMouseEvents()
    {
        Event evt = Event.current;
        Vector2 mousePos = evt.mousePosition;

        // 只在交互区域内处理鼠标事件
        if (!interactiveAreaRect.Contains(mousePos))
            return;

        Vector2 localPos = mousePos - interactiveAreaRect.position;

        switch (evt.type)
        {
            case EventType.MouseDown:
                Matrix4x4 transformMatrix = GetCurrentTransformMatrix();
                Vector3[] corners = TransformPoints(GetRectCorners(interactiveRect), transformMatrix);

                // 检查是否点击了控制点
                for (int i = 0; i < 4; i++)
                {
                    if (Vector2.Distance(localPos, corners[i]) < 8f)
                    {
                        isScaling = true;
                        dragStartPos = localPos;
                        originalRect = interactiveRect;
                        evt.Use();
                        return;
                    }
                }

                // 检查是否在矩形内部
                if (IsPointInPolygon(localPos, corners))
                {
                    isDragging = true;
                    dragStartPos = localPos;
                    originalRect = interactiveRect;
                    evt.Use();
                }
                break;

            case EventType.MouseDrag:
                if (isDragging)
                {
                    Vector2 delta = localPos - dragStartPos;
                    interactiveRect.position = originalRect.position + delta;
                    evt.Use();
                    Repaint();
                }
                else if (isScaling)
                {
                    Vector2 delta = localPos - dragStartPos;
                    interactiveRect.size = originalRect.size + delta;
                    evt.Use();
                    Repaint();
                }
                break;

            case EventType.MouseUp:
                isDragging = false;
                isScaling = false;
                break;
        }
    }

    void DrawTransformationDecomposition(Vector3[] originalCorners)
    {
        return;
        float t = Mathf.PingPong(animationTime * 3f, 1f);
        int phase = Mathf.FloorToInt(animationTime * 3f) % 3;

        Matrix4x4[] stepMatrices = new Matrix4x4[3];
        stepMatrices[0] = Matrix4x4.Translate(translation * t);
        stepMatrices[1] = stepMatrices[0] * Matrix4x4.Rotate(Quaternion.Euler(rotation * t));
        stepMatrices[2] = stepMatrices[1] * Matrix4x4.Scale(Vector3.Lerp(Vector3.one, scale, t));

        Color[] phaseColors = { Color.red, Color.green, Color.blue };

        for (int i = 0; i <= phase; i++)
        {
            Vector3[] stepCorners = TransformPoints(originalCorners, stepMatrices[i]);
            Handles.color = phaseColors[i] * new Color(1, 1, 1, 0.3f);

            for (int j = 0; j < 4; j++)
            {
                int next = (j + 1) % 4;
                Handles.DrawLine(stepCorners[j], stepCorners[next]);
            }
        }
    }

    void DrawGrid(Rect areaRect)
    {
        Handles.BeginGUI();
        Handles.color = new Color(1, 1, 1, 0.1f);

        for (int x = 20; x < areaRect.width; x += 20)
        {
            Handles.DrawLine(
                new Vector3(areaRect.x + x, areaRect.y, 0),
                new Vector3(areaRect.x + x, areaRect.y + areaRect.height, 0)
            );
        }

        for (int y = 20; y < areaRect.height; y += 20)
        {
            Handles.DrawLine(
                new Vector3(areaRect.x, areaRect.y + y, 0),
                new Vector3(areaRect.x + areaRect.width, areaRect.y + y, 0)
            );
        }

        Handles.EndGUI();
    }

    Matrix4x4 GetCurrentTransformMatrix()
    {
        Matrix4x4 translationMatrix = Matrix4x4.Translate(translation);
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(rotation));
        Matrix4x4 scaleMatrix = Matrix4x4.Scale(scale);

        Matrix4x4 shearMatrix = Matrix4x4.identity;
        shearMatrix[0, 1] = shearX;
        shearMatrix[1, 0] = shearY;

        return translationMatrix * rotationMatrix * scaleMatrix * shearMatrix;
    }

    Vector3[] GetRectCorners(Rect rect)
    {
        return new Vector3[]
        {
            new Vector3(rect.x, rect.y, 0),
            new Vector3(rect.x + rect.width, rect.y, 0),
            new Vector3(rect.x + rect.width, rect.y + rect.height, 0),
            new Vector3(rect.x, rect.y + rect.height, 0)
        };
    }

    Vector3[] TransformPoints(Vector3[] points, Matrix4x4 matrix)
    {
        Vector3[] result = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            // 转换为交互区域内的坐标
            Vector3 worldPoint = points[i] + new Vector3(interactiveAreaRect.position.x,interactiveAreaRect.position.y,0);
            result[i] = matrix.MultiplyPoint(worldPoint);
        }
        return result;
    }

    bool IsPointInPolygon(Vector2 point, Vector3[] polygon)
    {
        bool inside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
        {
            if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
            {
                inside = !inside;
            }
        }
        return inside;
    }

    void ResetTransform()
    {
        translation = Vector3.zero;
        rotation = Vector3.zero;
        scale = Vector3.one;
        shearX = 0f;
        shearY = 0f;
        interactiveRect = new Rect(50, 50, 100, 80);
        Repaint();
    }
}