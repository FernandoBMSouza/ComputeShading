using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    struct Cube //isso vai ser passado para GPU
    {
        //float3 = float[3] = Vector3(esse só existe na unity)
        public Vector3 position;
        //float4 = float[4] = Color(esse só existe na unity)
        //RGBA (red, green, blue and alpha)
        public Color color;
    }

    public ComputeShader computeShader;
    public int nCubes = 10;
    Cube[] data;
    GameObject[] gameObjects;
    public GameObject cubePrefab;

    private void OnGUI() 
    {
        if (GUI.Button(new Rect(0, 0, 100, 50), "Create"))
        {
            CreateCube();
        }

        if (GUI.Button(new Rect(110, 0, 100, 50), "Random CPU"))
        {
            //Fazendo em CPU a randomização de cores
            for (int i = 0; i < nCubes; i++)
            {
                for (int j = 0; j < nCubes; j++)
                {
                    gameObjects[i * nCubes + j].GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
                }
            }
        }

        if(GUI.Button(new Rect(220, 0, 100, 50), "Random GPU"))
        {
            //Fazendo a randomização de cores na GPU
            ProcessGPU();
        }
    }
    
    void CreateCube()
    {
        data = new Cube[nCubes * nCubes];
        gameObjects = new GameObject[nCubes * nCubes];

        for(int i = 0; i < nCubes; i++)
        {
            //setando a posicao X de cada cubo
            float offsetX = (-nCubes / 2 + i);

            for(int j = 0; j < nCubes; j++)
            {
                //setando a posicao Z de cada cubo
                float offsetZ = (-nCubes / 2 + j);

                GameObject go = GameObject.Instantiate(cubePrefab, 
                    new Vector3(offsetX * 1.1f, 0, offsetZ * 1.1f), //Essa multiplicação por 1.1f serve pra dar um espaçamento entre os cubos, para que não fiquem colados, é melhor multiplicar porque funciona pra qualquer size do cubo
                    Quaternion.identity);

                Color _colorInic = Random.ColorHSV();
                //Passando uma cor inicial aleatória para cada cubo
                go.GetComponent<MeshRenderer>().material.SetColor("_Color", _colorInic);
                
                //Passando os objetos intanciados para o array de gamebjects porque depois vamos precisar
                //usar esse array para trocar as cores
                //Esa conta (i * nCubes + j) é genial, serve para transformar dados de duas dimensões em uma dimensão 
                gameObjects[i * nCubes + j] = go;

                data[i * nCubes + j] = new Cube();
                data[i * nCubes + j].position = go.transform.position;
                data[i * nCubes + j].color = _colorInic;
            }
        }
    }

    void ProcessGPU()
    {
        //São 3 floats no vector3 + 4 floats no Color rgba, logo são 7 floats
        int totalBytes = sizeof(float) * 7;

        //data.Length é o mesmo que nCubes * nCubes
        ComputeBuffer cb = new ComputeBuffer(data.Length, totalBytes);
        cb.SetData(data); //envia dados da cpu para gpu

        //Esse método passa dados de memória de GPU para memória de CPU
        computeShader.SetBuffer(0, "cubes", cb);

        //Esses parametros é por causa do numThreads lá no computeshader, 
        //cada bloco processa 10 elementos por vez porque cada bloco tem 10 threads,
        //logo aqui ta dividindo a quantidade de elementos a serem processados pelo numero de threads de cada bloco,
        //encontrando assim a quantidade total de blocos necessários para processar todos os elementos
        computeShader.Dispatch(0, data.Length / 10, 1, 1); // esse método manda os dados pra GPU e volta
        
        //espera a gpu processar e te retorna os valores processados
        cb.GetData(data);

        for(int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].GetComponent<MeshRenderer>().material.SetColor("_Color", data[i].color);
        }

        cb.Dispose(); //Libera memória da GPU
    }
}


/*
Transformando de 1D para 2D:
for(i = 0 ... 100) {
    X = i % n
    Y = i / n
}

Transformando de 2D para 1D:
    i * nCubes + j
*/
