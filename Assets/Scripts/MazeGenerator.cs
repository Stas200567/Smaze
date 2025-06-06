// Scripts/MazeGenerator.cs
using UnityEngine;
using System.Collections.Generic;

// Цей клас не є MonoBehaviour, це просто структура даних.
public class MazeCell
{
    public bool Visited = false;
    // Стіни: true = існує, false = зруйнована
    public bool WallLeft = true;
    public bool WallBottom = true;
}

public class MazeGenerator : MonoBehaviour
{
    // Публічні поля для налаштування в інспекторі
    public Vector2Int Size { get; private set; } // Розмір сітки
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

    // Головний метод для запуску генерації
    public void Generate(Transform mazeContainer)
    {
        // 1. Очищуємо старий лабіринт
        foreach (Transform child in mazeContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. Ініціалізуємо клітинки
        cells = new MazeCell[Size.x, Size.y];
        for (int i = 0; i < Size.x; i++)
        {
            for (int j = 0; j < Size.y; j++)
            {
                cells[i, j] = new MazeCell();
            }
        }

        // 3. Запускаємо алгоритм "Recursive Backtracking"
        CarvePath(new Vector2Int(0, 0));

        // 4. Створюємо візуальні об'єкти стін
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
        // Перемішуємо напрямки для випадковості
        Shuffle(directions);

        foreach (var dir in directions)
        {
            Vector2Int neighborPos = position + dir;
            // Перевіряємо, чи сусід в межах поля і чи не відвіданий
            if (IsInRange(neighborPos) && !cells[neighborPos.x, neighborPos.y].Visited)
            {
                // Руйнуємо стіну між поточною і сусідньою клітинкою
                if (dir.x == 1) cells[position.x, position.y].WallLeft = false; // Йдемо праворуч, руйнуємо "праву" стіну поточної
                else if (dir.x == -1) cells[neighborPos.x, neighborPos.y].WallLeft = false; // Йдемо ліворуч, руйнуємо "праву" стіну лівого сусіда
                else if (dir.y == 1) cells[position.x, position.y].WallBottom = false; // Йдемо вгору, руйнуємо "верхню" стіну поточної
                else if (dir.y == -1) cells[neighborPos.x, neighborPos.y].WallBottom = false; // Йдемо вниз

                CarvePath(neighborPos);
            }
        }
    }

    private void DrawMaze(Transform mazeContainer)
    {
        // Зміщення, щоб лабіринт був у центрі (0,0)
        float offsetX = -Size.x / 2.0f + 0.5f;
        float offsetY = -Size.y / 2.0f + 0.5f;

        // Малюємо внутрішні стіни
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

        // Малюємо зовнішню рамку
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

    // Допоміжні функції
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