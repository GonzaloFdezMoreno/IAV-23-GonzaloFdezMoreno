/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Inform�tica de la Universidad Complutense de Madrid (Espa�a).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Navegacion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UCM.IAV.Movimiento
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;

        // Textos UI
        Text fRText;
        Text heuristicText;
        Text label;
        Text label2;
        string mazeSize = "10x10";

        private int frameRate = 60;
        TheseusGraph theseusGraph;

        // Variables de timer de framerate
        int m_frameCounter = 0;
        float m_timeCounter = 0.0f;
        float m_lastFramerate = 0.0f;
        float m_refreshTime = 0.5f;

        private bool cameraPerspective = true;

        GameObject player = null;
        GameObject exitSlab = null;
        GameObject startSlab = null;

        GameObject exit = null;
        GameObject startp = null;


        public GameObject[] checkpoints = new GameObject[4];
        GameObject guardia;
        

        int numMinos = 1;

        bool picked = false;
        bool dropped = true;
        float timedropped = 0.0f;
        public bool reach0 = true;
        public bool reach1 = false;
        public bool reach2 = false;
        public bool reach3 = false;

        private void Awake()
        {
            // Hacemos que el gestor del juego sea un Ejemplar �nico
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            Application.targetFrameRate = frameRate;

            FindGO();
        }

        private void OnLevelWasLoaded(int level)
        {
            FindGO();
        }

        // Update is called once per frame
        void Update()
        {
            if (guardia == null)
            {
                guardia = GameObject.Find("Guardia");
            }

            // Timer para mostrar el frameRate a intervalos
            if (m_timeCounter < m_refreshTime)
            {
                m_timeCounter += Time.deltaTime;
                m_frameCounter++;
            }
            else
            {
                m_lastFramerate = (float)m_frameCounter / m_timeCounter;
                m_frameCounter = 0;
                m_timeCounter = 0.0f;
            }

            // Texto con el framerate y 2 decimales
            if (fRText != null)
                fRText.text = (((int)(m_lastFramerate * 100 + .5) / 100.0)).ToString();

            if (player != null && (player.transform.position - exit.transform.position).magnitude < 0.5f && dropped && Input.GetKeyDown(KeyCode.E))
            {
                picked = true;
                dropped = false;
                //goToScene("Menu");
            }
            else if (picked && Input.GetKeyDown(KeyCode.E))
            {
                picked = false;
                dropped = true;
            }


            if (picked)
            {
                exitSlab.transform.position = player.transform.position;
                exit.transform.position = player.transform.position;
            }

            if (player != null && (player.transform.position - startp.transform.position).magnitude < 0.5f && picked) { 
                picked = false;
                goToScene("Menu");
            }

            if (guardia != null && (guardia.transform.position - checkpoints[0].transform.position).magnitude < 0.5f)
            {
                reach0 = true;
                reach1 = false;
                reach2 = false;
                reach3 = false;
            }

            if (guardia != null && (guardia.transform.position - checkpoints[1].transform.position).magnitude < 0.5f)
            {
                reach0 = false;
                reach1 = true;
                reach2 = false;
                reach3 = false;
            }

            if (guardia != null && (guardia.transform.position - checkpoints[2].transform.position).magnitude < 0.5f)
            {
                reach0 = false;
                reach1 = false;
                reach2 = true;
                reach3 = false;
            }

            if (guardia != null && (guardia.transform.position - checkpoints[3].transform.position).magnitude < 0.5f)
            {
                reach0 = false;
                reach1 = false;
                reach2 = false;
                reach3 = true;
            }

            

            //Input
            if (Input.GetKeyDown(KeyCode.R))
                RestartScene();
            if (Input.GetKeyDown(KeyCode.F))
                ChangeFrameRate();
            if (Input.GetKeyDown(KeyCode.C))
                heuristicText.text = theseusGraph.ChangeHeuristic();
        }

        private void FindGO()
        {
            if (SceneManager.GetActiveScene().name == "Menu") // Nombre de escena que habr�a que llevar a una constante
            {
                label = GameObject.FindGameObjectWithTag("DDLabel").GetComponent<Text>();
                label2 = GameObject.FindGameObjectWithTag("MinoLabel").GetComponent<Text>();
            }
            else if (SceneManager.GetActiveScene().name == "Labyrinth") // Nombre de escena que habr�a que llevar a una constante
            {
                fRText = GameObject.FindGameObjectWithTag("Framerate").GetComponent<Text>();
                heuristicText = GameObject.FindGameObjectWithTag("Heuristic").GetComponent<Text>();
                theseusGraph = GameObject.FindGameObjectWithTag("TesterGraph").GetComponent<TheseusGraph>();
                exitSlab = GameObject.FindGameObjectWithTag("Exit");
                startSlab = GameObject.FindGameObjectWithTag("Start");
                player = GameObject.Find("Avatar");
                
                
            }
        }

        public GameObject GetPlayer()
        {
            if (player == null) player = GameObject.Find("Avatar");
            return player;
        }

        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


        public void setNumMinos()
        {
            //numMinos = int.Parse(label2.text);
            numMinos = 1; 
        }

        public int getNumMinos()
        {
            return numMinos;
        }

        public void goToScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        public GameObject GetExitNode()
        {
            return exit;
        }

        public GameObject GetStartNode()
        {
            return startp;
        }

        public bool GetPicked()
        {
            return picked;
        }

        public bool isDropped()
        {
            return dropped;
        }

        public void SetExit(int i, int j, float size)
        {
            exit = new GameObject(); exit.name = "Exit";
            exit.transform.position = new Vector3(i * size, 0, j * size);
            exitSlab.transform.position = new Vector3(i * size, 0.3f, j * size);
        }

        public void SetStart(int i, int j, float size)
        {
            startp = new GameObject(); startp.name = "Start";
            startp.transform.position = new Vector3(i * size, 0, j * size);
            player.transform.position = new Vector3(i * size, 0.2f, j * size);
            startSlab.transform.position = new Vector3(i * size, 0.2f, j * size);

        }

        public void SetCheckPoint(int i, int j, float size,int order)
        {
            checkpoints[order] = new GameObject(); checkpoints[order].name = "Checkpoint"+order;
            checkpoints[order].transform.position = new Vector3(i * size, 0, j * size);


            
        }

        public GameObject GetCheckpointNode(int order)
        {
            
            return checkpoints[order];
        }

        public bool hasReachedfirstCheckpoint()
        {
            return reach0;
        }
        public bool hasReachedsecondCheckpoint()
        {
            return reach1;
        }

        public bool hasReachedthirdCheckpoint()
        {
            return reach2;
        }

        public bool hasReachedforthCheckpoint()
        {
            return reach3;
        }

        public GameObject GetGuardia()
        {
            return guardia;
        }


        private void ChangeFrameRate()
        {
            if (frameRate == 30)
            {
                frameRate = 60;
                Application.targetFrameRate = 60;
            }
            else
            {
                frameRate = 30;
                Application.targetFrameRate = 30;
            }
        }

        public void ChangeSize()
        {
            mazeSize = label.text;
        }
        public string getSize()
        {
            return mazeSize;
        }
    }
}