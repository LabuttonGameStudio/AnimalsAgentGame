using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseControl : MonoBehaviour
{
    //public Texture2D cursorTexture; // Textura para o cursor 
    public float maxDistance = 0.1f; // Maxima distancia do objeto 3D em relacao ao mouse
    public RectTransform canvasRect;
    public Transform mao;

    private void Start()
    {
        // Define o cursor do mouse como a imagem 
        //Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined; // Trava o cursor dentro da janela do jogo
  
    }

    private void Update()
    {
        // Obt�m a posicao do mouse na tela
        Vector3 mousePosition = Input.mousePosition;

        // Converte a posicao do mouse para o espa�o de mundo
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10));

            // Limita a distancia do objeto 3D em relacao ao mouse
           Vector3 targetPosition = Vector3.ClampMagnitude(worldPosition - transform.position, maxDistance) + transform.position;

            // Define a posi��o do objeto 3D
            mao.position = targetPosition;
      
    }
}
