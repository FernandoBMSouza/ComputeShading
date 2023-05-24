using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
    [SerializeField] private int nCubesPerLine = 10;
    [SerializeField] private GameObject cubePrefab;
    private GameObject[,] grid;
    private float updateInterval = 0.1f;
    bool flagStart = false;


    private void OnGUI() 
    {
        if (GUI.Button(new Rect(0, 0, 100, 50), "PLAY"))
        {
            flagStart = !flagStart;
            StartCoroutine(Simulate());
        }
    }
    
    void Start()
    {
        grid = new GameObject[nCubesPerLine, nCubesPerLine];

        for(int i = 0; i < nCubesPerLine; i++)
        {
            float offsetX = (-nCubesPerLine / 2 + i);
            for(int j = 0; j < nCubesPerLine; j++)
            {
                float offsetZ = (-nCubesPerLine / 2 + j);

                grid[i, j] = GameObject.Instantiate(cubePrefab, 
                                                    new Vector3(offsetX * 1.1f, 0, offsetZ * 1.1f),
                                                    Quaternion.identity);

                grid[i, j].GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
            }
        }
    }

    private int CountNeighbors(GameObject[,] grid, int row, int col)
    {
        int count = 0;
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int neighborRow = row + i;
                int neighborCol = col + j;

                // Verifica se as coordenadas do vizinho estão dentro dos limites do grid
                bool isValidNeighbor = (neighborRow >= 0 && neighborRow < rows) && (neighborCol >= 0 && neighborCol < cols);

                // Verifica se o vizinho é diferente do bloco em questão
                bool isDifferentBlock = (neighborRow != row) || (neighborCol != col);

                if (isValidNeighbor && isDifferentBlock 
                && grid[neighborRow, neighborCol].GetComponent<MeshRenderer>().material.color == Color.yellow)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void CheckRules()
    {
        for (int i = 0; i < nCubesPerLine; i++)
        {
            for (int j = 0; j < nCubesPerLine; j++)
            {
                if(CountNeighbors(grid, i, j) == 3)
                {
                    grid[i,j].GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
                }
                else if(grid[i, j].GetComponent<MeshRenderer>().material.color == Color.yellow 
                && (CountNeighbors(grid, i, j) < 2 || CountNeighbors(grid, i, j) > 3))
                {
                    grid[i,j].GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
                }
            }
        }
    }


    private IEnumerator Simulate()
    {
        while (flagStart)
        {
            CheckRules();
            yield return new WaitForSeconds(updateInterval);
        }
    }
}