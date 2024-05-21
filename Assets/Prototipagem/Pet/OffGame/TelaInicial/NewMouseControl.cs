using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMouseControl : MonoBehaviour
{
    public Transform mao; 

    private void Start()
    {
 
        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined; 
    }

    private void Update()
    {

        Vector3 mousePosition = Input.mousePosition;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.WorldToScreenPoint(mao.position).z));

        mao.position = worldPosition;
    }
}
