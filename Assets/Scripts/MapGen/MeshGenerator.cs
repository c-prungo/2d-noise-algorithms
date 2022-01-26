using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh (
        float[,] heightMap,
        int heightMultiplier,
        AnimationCurve heightCurve,
        bool includeTerrace,
        float terraceDetail,
        int levelOfDetail
    )
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftY = (height - 1) / 2f;

        int simplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / simplificationIncrement + 1;

        MeshData meshData = new MeshData (verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        for (int y = 0; y < height; y += simplificationIncrement)
        {
            for (int x = 0; x < width; x += simplificationIncrement)
            {
                // generate vertices
                float meshHeight = heightCurve.Evaluate(heightMap[x, y]);
                if (includeTerrace) {
                    meshHeight = Mathf.Round(meshHeight * terraceDetail) / terraceDetail;
                }
                meshHeight *= (float)heightMultiplier;
                meshData.vertices[vertexIndex] = new Vector3 (
                    x + topLeftX,
                    meshHeight,
                    topLeftY - y
                );

                // generate uvs
                meshData.uvs[vertexIndex] = new Vector2 (
                    x/(float)width,
                    y/(float)height
                );

                // generate triangles
                if ((y < height - 1) && (x < width - 1))
                {
                    meshData.AddTriangle (
                        vertexIndex,
                        vertexIndex + verticesPerLine + 1,
                        vertexIndex + verticesPerLine
                    );
                    meshData.AddTriangle(
                        vertexIndex + verticesPerLine + 1,
                        vertexIndex,
                        vertexIndex + 1
                    );
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex = 0;

    public MeshData (int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        uvs = new Vector2[meshWidth * meshHeight];
    }

    public void AddTriangle (int a, int b, int c)
    {
        triangles [triangleIndex] = a;
        triangles [triangleIndex+1] = b;
        triangles [triangleIndex+2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh ()
    {
        Mesh mesh = new Mesh ();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals ();
        return mesh;
    }
}