using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController_Motor : MonoBehaviour
{

    public float speed = 10.0f;
    public float sensitivity = 30.0f;
    public float WaterHeight = 15.5f;
    CharacterController character;
    public GameObject cam;
    float moveFB, moveLR;
    float rotX, rotY;
    public bool webGLRightClickRotation = true;
    float gravity = -9.8f;
    //new
    float jumpforce = 5f;
    float verticalvelocity = 0;
    float gravityscale = 20f;

    // 新增 pitch 變數
    private float pitch = 0f;

    void Start()
    {
        character = GetComponent<CharacterController>();
        if (Application.isEditor)
        {
            webGLRightClickRotation = false;
            sensitivity = sensitivity * 1.5f;
        }
        // 初始化 pitch 為目前相機的本地 X 角度
        if (cam != null)
            pitch = cam.transform.localEulerAngles.x;
    }


    void CheckForWaterHeight()
    {
        if (transform.position.y < WaterHeight)
        {
            gravity = 0f;
        }
        else
        {
            gravity = -9.8f;
        }
    }

    void Update()
    {
        moveFB = Input.GetAxis("Horizontal") * speed;
        moveLR = Input.GetAxis("Vertical") * speed;

        rotX = Input.GetAxis("Mouse X") * sensitivity;
        rotY = Input.GetAxis("Mouse Y") * sensitivity;

        CheckForWaterHeight();
        if (character.isGrounded)
        {
            verticalvelocity = -1f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalvelocity = jumpforce;
            }
        }
        else
            verticalvelocity += gravity * Time.deltaTime;

        Vector3 movement = new Vector3(moveFB, verticalvelocity, moveLR);

        if (webGLRightClickRotation)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                CameraRotation(cam, rotX, rotY);
            }
        }
        else if (!webGLRightClickRotation)
        {
            CameraRotation(cam, rotX, rotY);
        }

        movement = transform.rotation * movement;
        character.Move(movement * Time.deltaTime);
    }

    void CameraRotation(GameObject cam, float rotX, float rotY)
    {
        transform.Rotate(0, rotX * Time.deltaTime, 0);

        // 累加 pitch，限制在 -65 到 65 度之間
        pitch -= rotY * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -65f, 65f);

        // 設定相機本地 X 角度
        Vector3 localEuler = cam.transform.localEulerAngles;
        // 由於角度會繞 360 度，需要適當處理
        localEuler.x = pitch;
        cam.transform.localEulerAngles = new Vector3(pitch, localEuler.y, localEuler.z);
    }
}