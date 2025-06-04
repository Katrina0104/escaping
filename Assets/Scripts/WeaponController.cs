using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    // Fire
    public Transform bulletStartPoint;
    public float fireRate = 0.1f;
    public bool isFire = false;
    public int magazineSize = 30;          // 單個彈夾容量
    public int bulletamount = 30;          // 當前彈夾子彈
    //public int totalAmmo = 90;             // 總備用子彈
    public GameObject[] Effects; // Effects[0]: 槍口特效, Effects[1]: 命中特效

    // 彈孔Prefab(命中圖案)
    public GameObject hitDecalPrefab;
    public LayerMask decalLayerMask = ~0; // Inspector可調整

    // Weapon shooting back 
    public Transform defaultPoint;
    public Transform backPoint;
    public float lerpRatio = 0.2f;

    // Raycast
    public float range = 100f;
    public float damage = 10f;

    // VeiwControl
    public Camera mainCamera;
    public Camera weaponCamera;
    public Vector3 weaponCameraDefaultPoint;
    public Vector3 weaponCameraCenterPoint;
    public float defaultVeiw = 60;
    public float cenetrVeiw = 30;
    public float viewLerpRatio = 0.2f;

    // CrossHair
    [Header("Crosshair")]
    public Texture2D crosshairTexture;      // 準心圖片，Inspector 指定
    public int crosshairSize = 16;          // 準心尺寸
    private float currentCrosshairSize;           // 當前尺寸（動態變化）
    public float crosshairZoomSize = 50;          // 瞄準時放大到多大
    public float crosshairResizeSpeed = 10f;      // 變化速度

    // Music
    public AudioClip shotSound;
    public AudioClip reloadSound; // 換彈音效

    //鼠標消失
    private bool isPaused = false;

    //bullet reload
    private bool isReloading = false;

    // Weapon Camera recoil
    [Header("Weapon Camera Recoil")]
    public float recoilRotationAmount = 3f;         // 每次開槍往上彈的角度
    public float recoilReturnSpeed = 8f;            // 回正速度
    private float currentRecoil = 0f;               // 當前recoil角度

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        weaponCamera = GameObject.FindGameObjectWithTag("WeaponCamera").GetComponent<Camera>();
        bulletamount = magazineSize;

        currentCrosshairSize = crosshairSize;

        SetMouseLock(true);
    }

    void Update()
    {
        // 如果正在換彈，不允許射擊
        if (isReloading) return;

        OnFire();
        VeiwChange();
        HandleWeaponCameraRecoil();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            SetMouseLock(!isPaused);

            // 這裡可加暫停菜單、暫停遊戲等
        }

        // 按下R鍵換彈
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (bulletamount < magazineSize)   // 如果之後要限制子彈數量   && totalAmmo > 0
            {
                StartCoroutine(Reload());
            }
        }

        // crosshair 尺寸漸漸回到預設
        currentCrosshairSize = Mathf.Lerp(currentCrosshairSize, crosshairSize, Time.deltaTime * crosshairResizeSpeed);
    }

    void OnGUI()    // 準心繪製
    {
        if (crosshairTexture != null)
        {
            float xMin = (Screen.width - currentCrosshairSize) / 2;
            float yMin = (Screen.height - currentCrosshairSize) / 2;
            GUI.DrawTexture(new Rect(xMin, yMin, currentCrosshairSize, currentCrosshairSize), crosshairTexture);
        }
    }

    private void OnFire()
    {
        if (Input.GetMouseButtonDown(0) && !isReloading)
        {
            isFire = true;
            StartCoroutine(Fire());
        }

        if (Input.GetMouseButtonUp(0))
        {
            isFire = false;
            StopCoroutine("Fire");
        }
    }

    IEnumerator Fire()
    {
        while (isFire && bulletamount > 0 && !isReloading)
        {
            // 槍口特效
            if (Effects != null && Effects.Length > 0 && Effects[0] != null && bulletStartPoint != null)
            {
                Instantiate(Effects[0], bulletStartPoint.position, bulletStartPoint.rotation);
            }

            // 射擊音效
            PlayShotAudio();

            // 槍托後座動畫
            StartCoroutine(WeaponBackAnimation());

            // Weapon Camera recoil
            currentRecoil += recoilRotationAmount;

            // 扣除子彈
            bulletamount--;

            // Raycast 判斷命中
            if (bulletStartPoint != null)
            {
                RaycastHit hit;
                Vector3 shootDirection = bulletStartPoint.forward;

                if (Physics.Raycast(bulletStartPoint.position, shootDirection, out hit, range))
                {
                    Debug.Log("Raycast Hit: " + hit.collider.name);

                    // 給予傷害，如果對方有 TakeDamage 函數
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        hit.collider.GetComponent<EnemyHealth>().TakeDamage(25);
                        //EnemyHealth.Instance.TakeDamage(25);
                    }

                    // 命中特效
                    if (Effects != null && Effects.Length > 1 && Effects[1] != null)
                    {
                        Instantiate(Effects[1], hit.point, Quaternion.LookRotation(hit.normal));
                    }

                    // 命中圖案/彈孔
                    if (hit.collider != null && !hit.collider.isTrigger &&
                        ((1 << hit.collider.gameObject.layer) & decalLayerMask) != 0)
                    {
                        GameObject decal = Instantiate(
                            hitDecalPrefab,
                            hit.point + hit.normal * 0.01f,
                            Quaternion.FromToRotation(Vector3.up, hit.normal)
                        );
                        decal.transform.SetParent(hit.collider.transform);
                    }
                }
            }

            yield return new WaitForSeconds(fireRate);

            // 自動換彈
            if (bulletamount <= 0)     //如果之後要限制備用子彈 && totalAmmo > 0
            {
                StartCoroutine(Reload());
                yield break;
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        isFire = false;

        PlayReloadAudio();

        // 可依reloadSound長度或自訂換彈時間
        float reloadTime = reloadSound ? reloadSound.length : 2f;
        yield return new WaitForSeconds(reloadTime);

        bulletamount = magazineSize; //永遠填滿

        //int neededAmmo = magazineSize - bulletamount;
        //int ammoToLoad = Mathf.Min(neededAmmo, totalAmmo);

        //bulletamount += ammoToLoad;
        //totalAmmo -= ammoToLoad;

        isReloading = false;
    }

    IEnumerator WeaponBackAnimation()
    {
        if (defaultPoint != null && backPoint != null)
        {
            // back
            while (this.transform.localPosition != backPoint.localPosition)
            {
                this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, backPoint.localPosition, lerpRatio * 4);
                yield return null;
            }
            // go back to default
            while (this.transform.localPosition != defaultPoint.localPosition)
            {
                this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, defaultPoint.localPosition, lerpRatio * 4);
                yield return null;
            }
        }
    }

    private void PlayShotAudio()
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (shotSound)
        {
            audio.PlayOneShot(shotSound);
        }
    }

    private void PlayReloadAudio()
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (reloadSound)
        {
            audio.PlayOneShot(reloadSound);
        }
    }

    private void VeiwChange()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StopCoroutine("VeiwToDefault");
            StartCoroutine("VeiwToCenter");
            currentCrosshairSize = crosshairZoomSize; // 放大crosshair
        }
        if (Input.GetMouseButtonUp(1))
        {
            StopCoroutine("VeiwToCenter");
            StartCoroutine("VeiwToDefault");
        }
    }
    IEnumerator VeiwToCenter()
    {
        while (weaponCamera.transform.localPosition != weaponCameraCenterPoint)
        {
            weaponCamera.transform.localPosition = Vector3.Lerp(weaponCamera.transform.localPosition, weaponCameraCenterPoint, viewLerpRatio);
            weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, cenetrVeiw, viewLerpRatio);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, cenetrVeiw, viewLerpRatio);
            yield return null;
        }
    }
    IEnumerator VeiwToDefault()
    {
        while (weaponCamera.transform.localPosition != weaponCameraDefaultPoint)
        {
            weaponCamera.transform.localPosition = Vector3.Lerp(weaponCamera.transform.localPosition, weaponCameraDefaultPoint, viewLerpRatio);
            weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, defaultVeiw, viewLerpRatio);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, defaultVeiw, viewLerpRatio);
            yield return null;
        }
    }
    private void SetMouseLock(bool isLock)
    {
        if (isLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Weapon Camera recoil effect handler
    private void HandleWeaponCameraRecoil()
    {
        if (weaponCamera != null)
        {
            // 只在 recoiling 時才施加
            currentRecoil = Mathf.Lerp(currentRecoil, 0, Time.deltaTime * recoilReturnSpeed);
            weaponCamera.transform.localRotation = Quaternion.Euler(-currentRecoil, 0, 0);
        }
    }
}