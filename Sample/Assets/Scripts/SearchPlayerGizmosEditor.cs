using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SearchPlayerGizmosEditor : MonoBehaviour
{
    private static readonly int TRIANGLE_COUNT = 12;
    private static readonly Color MESH_COLOR = new Color(1.0f,1.0f,0.0f,0.7f);

    //gizmoを描画
    [DrawGizmo(GizmoType.NonSelected|GizmoType.Selected)]
    private static void DrawPointGizmos(SearchPlayer searchObj , GizmoType gizmoType)
    {
        if (searchObj.SearchRadius <= 0.0f)
        {
            return;
        }
        Gizmos.color = MESH_COLOR;
        Transform transform = searchObj.transform;
        //地面と同じ高さだとgizmoが見えにくいため少しposを上に
        Vector3 pos = transform.position + Vector3.up * 0.01f;
        Quaternion rot = transform.rotation;
        Vector3 scale = Vector3.one * searchObj.SearchRadius;
        if (searchObj.SearchAngle > 0.0f)
        {
            Mesh fanMesh = CreateFanMesh(searchObj.SearchAngle, TRIANGLE_COUNT);
            Gizmos.DrawMesh(fanMesh, pos, rot, scale);
        }
    }


    //扇形のメッシュを作成
    private static Mesh CreateFanMesh(float angle ,int triangleCount)
    {
        var mesh = new Mesh();

        var vertices  =    CreateFanVertices(angle,triangleCount);

        var triangleIndexes = new List<int>(triangleCount*3);

        for(int i = 0; i < triangleCount; ++i)
        {
            triangleIndexes.Add(0);
            triangleIndexes.Add(i + 1);
            triangleIndexes.Add(i + 2);
        }

        //頂点配列作成
        mesh.vertices = vertices;
        //ポリゴンインデックス配列作成
        mesh.triangles = triangleIndexes.ToArray();


        mesh.RecalculateNormals();

        return mesh;
    }


    //扇形メッシュの頂点配列を作成
    private static Vector3[] CreateFanVertices (float angle, int triangleCount)
    {
        if(angle <= 0)
        {
            throw new System.AccessViolationException(string.Format("角度がおかしい angle={0}",angle));
        }
        if(triangleCount <= 0)
        {
            throw new System.AccessViolationException(string.Format("数がおかしい triangleCount={0}",triangleCount));
        }
        angle = Mathf.Min(angle,360.0f);

        var vertices = new List<Vector3>(triangleCount + 2);

        //最初の頂点を追加
        vertices.Add(Vector3.zero);

        //頂点位置を計算
        float radian   = angle * Mathf.Deg2Rad;
        float startRad = -radian / 2;
        float incRad = radian / triangleCount;
        
        for(int i = 0; i < triangleCount + 1; ++i)
        {
            float currentRad = startRad + (incRad * i);
            Vector3 vertex = new Vector3(Mathf.Sin(currentRad),0.0f,Mathf.Cos(currentRad));
            //計算した頂点を追加
            vertices.Add(vertex);
        }
        return vertices.ToArray();
    }
}
