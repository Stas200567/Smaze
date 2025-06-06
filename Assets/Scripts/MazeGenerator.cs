// Scripts/MazeGenerator.cs
using UnityEngine;
using System.Collections.Generic;

// ��� ���� �� � MonoBehaviour, �� ������ ��������� �����.
public class MazeCell
{
    public bool Visited = false;
    // ����: true = ����, false = ����������
    public bool WallLeft = true;
    public bool WallBottom = true;
}

public class MazeGenerator : MonoBehaviour
{
    // ������ ���� ��� ������������ � ���������
    public Vector2Int Size { get; private set; } // ����� ����
    public GameObject WallPrefab;

    private MazeCell[,] cells;

    public void SetDifficulty(int difficulty) // 0: easy, 1: medium, 2: hard
    {
        switch (difficulty)
        {
            case 1: Size = new Vector2Int(15, 15); break;
            case 2: Size = new Vector2Int(20, 20); break;
            default: Size = new Vector2Int(10, 10); break;
        }
    }

    // �������� ����� ��� ������� ���������
    public void Generate(Transform mazeContainer)
    {
        // 1. ������� ������ �������
        foreach (Transform child in mazeContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. ���������� �������
        cells = new MazeCell[Size.x, Size.y];
        for (int i = 0; i < Size.x; i++)
        {
            for (int j = 0; j < Size.y; j++)
            {
                cells[i, j] = new MazeCell();
            }
        }

        // 3. ��������� �������� "Recursive Backtracking"
        CarvePath(new Vector2Int(0, 0));

        // 4. ��������� ������� ��'���� ���
        DrawMaze(mazeContainer);
    }

    private void CarvePath(Vector2Int position)
    {
        cells[position.x, position.y].Visited = true;
        Vector2Int[] directions = {
            new Vector2Int(0, 1),  // Up
            new Vector2Int(1, 0),  // Right
            new Vector2Int(0, -1), // Down
            new Vector2Int(-1, 0)  // Left
        };
        // ��������� �������� ��� �����������
        Shuffle(directions);

        foreach (var dir in directions)
        {
            Vector2Int neighborPos = position + dir;
            // ����������, �� ���� � ����� ���� � �� �� ��������
            if (IsInRange(neighborPos) && !cells[neighborPos.x, neighborPos.y].Visited)
            {
                // ������� ���� �� �������� � �������� ��������
                if (dir.x == 1) cells[position.x, position.y].WallLeft = false; // ����� ��������, ������� "�����" ���� �������
                else if (dir.x == -1) cells[neighborPos.x, neighborPos.y].WallLeft = false; // ����� ������, ������� "�����" ���� ����� �����
                else if (dir.y == 1) cells[position.x, position.y].WallBottom = false; // ����� �����, ������� "������" ���� �������
                else if (dir.y == -1) cells[neighborPos.x, neighborPos.y].WallBottom = false; // ����� ����

                CarvePath(neighborPos);
            }
        }
    }

    private void DrawMaze(Transform mazeContainer)
    {
        // �������, ��� ������� ��� � ����� (0,0)
        float offsetX = -Size.x / 2.0f + 0.5f;
        float offsetY = -Size.y / 2.0f + 0.5f;

        // ������� ������� ����
        for (int i = 0; i < Size.x; i++)
        {
            for (int j = 0; j < Size.y; j++)
            {
                Vector3 cellPos = new Vector3(i + offsetX, j + offsetY, 0);

                if (cells[i, j].WallLeft)
                {
                    Transform wall = Instantiate(WallPrefab, mazeContainer).transform;
                    wall.localPosition = cellPos + new Vector3(0.5f, 0, 0);
                    wall.localRotation = Quaternion.Euler(0, 90, 0);
                    wall.localScale = new Vector3(1, 1, 0.1f);
                    wall.gameObject.tag = "Wall";
                }
                if (cells[i, j].WallBottom)
                {
                    Transform wall = Instantiate(WallPrefab, mazeContainer).transform;
                    wall.localPosition = cellPos + new Vector3(0, -0.5f, 0);
                    wall.localRotation = Quaternion.identity;
                    wall.localScale = new Vector3(1, 1, 0.1f);
                    wall.gameObject.tag = "Wall";
                }
            }
        }

        // ������� ������� �����
        Transform topWall = Instantiate(WallPrefab, mazeContainer).transform;
        topWall.localPosition = new Vector3(offsetX + (Size.x - 1) / 2.0f, offsetY + Size.y - 0.5f, 0);
        topWall.localScale = new Vector3(Size.x, 1, 0.1f);
        topWall.gameObject.tag = "Wall";

        Transform leftWall = Instantiate(WallPrefab, mazeContainer).transform;
        leftWall.localPosition = new Vector3(offsetX - 0.5f, offsetY + (Size.y - 1) / 2.0f, 0);
        leftWall.localRotation = Quaternion.Euler(0, 90, 0);
        leftWall.localScale = new Vector3(Size.y, 1, 0.1f);
        leftWall.gameObject.tag = "Wall";
    }

    // ������� �������
    private bool IsInRange(Vector2Int pos) => pos.x >= 0 && pos.x < Size.x && pos.y >= 0 && pos.y < Size.y;
    private void Shuffle<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int rand = Random.Range(i, array.Length);
            T temp = array[i];
            array[i] = array[rand];
            array[rand] = temp;
        }
    }
}