using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    [SerializeField] private int nCube = 20; // Tamanho do grid na direção X e Y
    [SerializeField] private GameObject cellPrefab; // Prefab da célula
    [SerializeField] private float timeInterval = 0.5f;
    private float timer = 0; // Intervalo de tempo entre as gerações
    private bool flagStart;
    private bool[,] grid;
    private GameObject[,] cells;

    //Parte do processo de GPU
    Cell[] celldata;

    private void Start()
    {
        // Inicializa o grid e as células
        grid = new bool[nCube, nCube];
        cells = new GameObject[nCube, nCube];

        // Posição inicial do grid
        Vector3 gridOrigin = new Vector3(-nCube / 2f, -nCube / 2f, 0f);

        // Cria as células do grid
        for(int i = 0; i < nCube; i++)
        {
            float offsetX = (-nCube / 2 + i);
            for(int j = 0; j < nCube; j++)
            {
                float offsetZ = (-nCube / 2 + j);

                cells[i, j] = GameObject.Instantiate(cellPrefab, 
                                                    new Vector3(offsetX * 1.1f, 0, offsetZ * 1.1f),
                                                    Quaternion.identity);

                cells[i, j].GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);

                
            }
        }
    }

    private void Update() 
    {
        if(flagStart)
        {
            timer += Time.deltaTime;

            if (timer >= timeInterval)
            {
                UpdateGeneration();
                timer = 0f;
            }  
        }
    }

    private void SetInitialStates()
    {
         for (int x = 0; x < nCube; x++)
        {
            for (int y = 0; y < nCube; y++)
            {
                if(isAlive(cells[x, y]))
                    grid[x, y] = true;
                else
                    grid[x, y] = false;
            }
        }
    }

    private void OnGUI() 
    {
        if (GUI.Button(new Rect(0, 0, 100, 50), "PLAY / STOP"))
        {
            SetInitialStates();
            flagStart = !flagStart;
            // Inicia o processo de atualização das gerações
            //InvokeRepeating("UpdateGeneration", 0f, timeInterval);
        }
    }

    private void UpdateGeneration()
    {
        // Calcula a próxima geração
        bool[,] nextGeneration = new bool[nCube, nCube]; //cria uma matriz do mesmo tamanho só que de booleans

        for (int x = 0; x < nCube; x++)
        {
            for (int y = 0; y < nCube; y++)
            {
                int aliveNeighbors = CountAliveNeighbors(x, y); //pega a quantidade de vizinhos

                if (grid[x, y])
                {
                    // Se uma célula viva tem menos de 2 ou mais de 3 vizinhos vivos, ela morre
                    nextGeneration[x, y] = (aliveNeighbors == 2 || aliveNeighbors == 3);
                }
                else
                {
                    // Se uma célula morta tem exatamente 3 vizinhos vivos, ela se torna viva
                    nextGeneration[x, y] = (aliveNeighbors == 3);
                }
            }
        }

        // Atualiza o grid e as células com a próxima geração
        grid = nextGeneration;

        for (int x = 0; x < nCube; x++)
        {
            for (int y = 0; y < nCube; y++)
            {
                //cells[x, y].SetActive(grid[x, y]);
                if(grid[x, y])
                    Born(cells[x,y]);
                else
                    Kill(cells[x,y]);
            }
        }
    }

    private int CountAliveNeighbors(int x, int y)
    {
        int count = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // Ignora a célula atual
                if (i == 0 && j == 0)
                    continue;

                int neighborX = x + i;
                int neighborY = y + j;

                // Verifica se o vizinho está dentro dos limites do grid
                if (neighborX >= 0 && neighborX < nCube && neighborY >= 0 && neighborY < nCube)
                {
                    if (grid[neighborX, neighborY])
                        count++;
                }
            }
        }

        return count;
    }

    private bool isAlive(GameObject cube)
    {
        if(cube.GetComponent<MeshRenderer>().material.color == Color.yellow)
            return true;
        else
            return false;
    }

    private void Born(GameObject cube)
    {
        cube.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
    }

    private void Kill(GameObject cube)
    {
        cube.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
    }

#region GPUTestes

    struct Cell {
        public Vector3 position;
        public Color color;
        public Color nextColor;
    }

    private void ProcessGPU()
    {

    }
#endregion
}
