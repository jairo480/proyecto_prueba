using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    public Transform personaje;
    public float suavidad = 0.1f;

    private float tamañoCamara;
    private float alturaPantalla;

    void Start()
    {
        tamañoCamara = Camera.main.orthographicSize;
        alturaPantalla = tamañoCamara * 2;
    }

    void Update()
    {
        CalcularPosicionCamara();
    }

    void CalcularPosicionCamara()
    {
        int pantallaPersonajeY = (int)(personaje.position.y / alturaPantalla);
        int pantallaPersonajeX = (int)(personaje.position.x / alturaPantalla);

        float alturaCamara = (pantallaPersonajeY * alturaPantalla) + tamañoCamara;
        float xCamara = (pantallaPersonajeX * alturaPantalla) + tamañoCamara;

        // Calcula la nueva posición de la cámara y realiza una interpolación suave
        Vector3 nuevaPosicionCamara = new Vector3(xCamara, alturaCamara, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, nuevaPosicionCamara, suavidad);
    }
}