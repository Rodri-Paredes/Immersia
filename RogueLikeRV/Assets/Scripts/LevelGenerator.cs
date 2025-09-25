using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject startRoomPrefab; // Prefab del cuarto de inicio
    public GameObject connectionRoomPrefab; // Prefab del cuarto de conexión
    public GameObject standardRoomPrefab; // Prefab del cuarto estándar
    public GameObject endRoomPrefab; // Prefab del cuarto final
    public GameObject extraRoomPrefab; // Prefab del cuarto extra para exploración
    public GameObject boundaryRoomPrefab; // Prefab de cuarto con colliders para los bordes
    public GameObject generationPoint; // GameObject vacío para definir el punto de generación
    public int numberOfStandardRooms = 6; // Número de salas estándar a generar
    public int numberOfExtraRooms = 4; // Número de salas extra para exploración
    public float roomWidth = 10f; // Ancho de las salas
    public float roomDepth = 10f; // Profundidad de las salas

    private Vector3 currentPosition;
    private List<Vector3> usedPositions;
    private Vector3 highestRoomPosition;

    void Start()
    {
        usedPositions = new List<Vector3>();
        currentPosition = generationPoint.transform.position; // Usar la posición del GameObject como punto de partida
        highestRoomPosition = currentPosition;
        GenerateLevel();
        SurroundWithBoundaryRooms();
    }

    void GenerateLevel()
    {
        // Generar el cuarto de inicio
        Instantiate(startRoomPrefab, currentPosition, Quaternion.identity);
        usedPositions.Add(currentPosition);

        // Generar el primer cuarto de conexión a la izquierda del cuarto de inicio
        currentPosition += new Vector3(-roomWidth, 0, 0);
        Instantiate(connectionRoomPrefab, currentPosition, Quaternion.identity);
        usedPositions.Add(currentPosition);

        // Generar un cuarto estándar a la izquierda del cuarto de conexión
        currentPosition += new Vector3(-roomWidth, 0, 0);
        Instantiate(standardRoomPrefab, currentPosition, Quaternion.identity);
        usedPositions.Add(currentPosition);
        highestRoomPosition = currentPosition; // Actualizar la posición más alta

        // Generar más cuartos de conexión y estándar
        for (int i = 0; i < numberOfStandardRooms - 2; i++)
        {
            Vector3 newPosition = GetNextRoomPosition();
            Instantiate(standardRoomPrefab, newPosition, Quaternion.identity);
            usedPositions.Add(newPosition);

            // Actualizar la posición más alta si es necesario
            if (newPosition.z > highestRoomPosition.z)
            {
                highestRoomPosition = newPosition;
            }

            // Añadir ramificaciones extras desde cuartos estándar o de conexión
            if (i % 2 == 0)
            {
                Vector3 extraPosition = GetNextRoomPosition();
                Instantiate(extraRoomPrefab, extraPosition, Quaternion.identity);
                usedPositions.Add(extraPosition);
            }
        }

        // Generar el cuarto final en la posición después del cuarto más alto
        Vector3 endRoomPosition = highestRoomPosition + new Vector3(0, 0, roomDepth);
        Instantiate(endRoomPrefab, endRoomPosition, Quaternion.identity);
        usedPositions.Add(endRoomPosition);

        // Generar cuartos extra para exploración
        for (int i = 0; i < numberOfExtraRooms; i++)
        {
            Vector3 extraPosition = GetNextRoomPosition();
            Instantiate(extraRoomPrefab, extraPosition, Quaternion.identity);
            usedPositions.Add(extraPosition);
        }
    }

    void SurroundWithBoundaryRooms()
    {
        // Crear una capa de cuartos con colliders alrededor del nivel
        Vector3[] boundaryOffsets = new Vector3[]
        {
            new Vector3(roomWidth, 0, 0), // Derecha
            new Vector3(-roomWidth, 0, 0), // Izquierda
            new Vector3(0, 0, roomDepth), // Arriba
            new Vector3(0, 0, -roomDepth) // Abajo
        };

        List<Vector3> boundaryPositions = new List<Vector3>();
        foreach (Vector3 pos in usedPositions)
        {
            foreach (Vector3 offset in boundaryOffsets)
            {
                Vector3 boundaryPosition = pos + offset;
                if (!usedPositions.Contains(boundaryPosition) && !boundaryPositions.Contains(boundaryPosition))
                {
                    Instantiate(boundaryRoomPrefab, boundaryPosition, Quaternion.identity);
                    boundaryPositions.Add(boundaryPosition);
                }
            }
        }
    }

    Vector3 GetNextRoomPosition()
    {
        Vector3[] possibleDirections = new Vector3[]
        {
            new Vector3(-roomWidth, 0, 0), // Mover a la izquierda
            new Vector3(roomWidth, 0, 0), // Mover a la derecha
            new Vector3(0, 0, roomDepth), // Mover hacia adelante/arriba
            new Vector3(0, 0, -roomDepth) // Mover hacia atrás/abajo
        };

        Vector3 direction = possibleDirections[Random.Range(0, possibleDirections.Length)];
        Vector3 newPosition = currentPosition + direction;

        // Verificar que la nueva posición no colida con posiciones usadas
        while (usedPositions.Contains(newPosition))
        {
            direction = possibleDirections[Random.Range(0, possibleDirections.Length)];
            newPosition = currentPosition + direction;
        }

        currentPosition = newPosition; // Actualizar la posición actual
        return newPosition;
    }
}
