using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private AmmoBar ammoBar; 
    [SerializeField] private CurrentWeapon currentWeaponUI;



    // Variabler
    float speed = 10f;
    KeyCode shootKey = KeyCode.Space;
    public Freezeframe freezeFrame;
    public CameraShake screenShake;

    //Vapen Variabler
    [SerializeField] private Weapon glockPrefab, sniperPrefab, RPGPrefab, SMGPrefab;
    public Powerup powerupPrefab;
    Weapon currentWeapon;
    [SerializeField] private AudioSource weaponSoundEffect;
    [SerializeField] private AudioSource emptyAmmo;
    bool canShoot = true;
    bool waiting = false;
    //float maxAmmoTemp = 10f;


    /* What each weapon needs (skapa prefabs som �rver)
     * Damage
     * Rate of fire
     * Max ammo 
    */

    public void TestWeaponUI()  
    {
        
    }

    private void Start()
    {
        SwapWeapon(glockPrefab);
        ammoBar.SetMaxAmmo((float)currentWeapon.baseAmmo); //nn 
        currentWeaponUI.UpdateWeaponUI(currentWeapon);//nnn 
    }

    // Update is called once per frame
    void Update()
    {
        TestWeaponUI(); 

        currentWeaponUI.UpdateWeaponUI(currentWeapon); //nnn 
        ammoBar.SetMaxAmmo((float)currentWeapon.baseAmmo);
        ammoBar.SetAmmo(currentWeapon.ammo);


        Vector3 position = transform.position;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            position.x -= speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            position.x += speed * Time.deltaTime;
        }

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        position.x = Mathf.Clamp(position.x, leftEdge.x, rightEdge.x);

        transform.position = position;
        

        if (Input.GetKey(shootKey))
        {
            Shoot(currentWeapon.ammo, currentWeapon.fireRate, currentWeapon.damage, currentWeapon.projectileSpeed);
        }

        if (Application.isEditor)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                SwapWeapon(glockPrefab);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                SwapWeapon(sniperPrefab);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SwapWeapon(SMGPrefab);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SwapWeapon(RPGPrefab);
            }
        }
    }
    private void Shoot(int ammo, float fireRate, float damage, float projectileSpeed)
    {
        if (canShoot = true && !waiting)//Kollar om coroutinen Cooldown k�r med hj�lp av waiting variabeln, s� att vi inte startar flera cooldowns.
        {
            if (currentWeapon.ammo <= 0)
            {
                ResetWeapon();
                emptyAmmo.PlayOneShot(emptyAmmo.clip, 0.3f);
            }
            currentWeapon.SpawnProjectile();
            StartCoroutine(screenShake.Shake(0.25f, 0.10f));
            StartCoroutine(Cooldown(fireRate));  
            currentWeapon.ammo -= 1;
            if(currentWeapon is RPG)
            {
                weaponSoundEffect.PlayOneShot(weaponSoundEffect.clip, 0.25f);
            } else if(currentWeapon is SMG)
            {
                weaponSoundEffect.PlayOneShot(weaponSoundEffect.clip, 0.05f);
            } 
            else
            {
                weaponSoundEffect.PlayOneShot(weaponSoundEffect.clip, 0.10f);
            }
        }
    }

    private void ResetWeapon()
    {
        SwapWeapon(glockPrefab);
    }

    private void SwapWeapon(Weapon newWeapon)
    {
        if(currentWeapon != null)
        {
            currentWeapon.removeObject();
        }
        currentWeapon = Instantiate(newWeapon, newWeapon.transform.position, newWeapon.transform.rotation, transform); 

        ammoBar.SetMaxAmmo(currentWeapon.ammo); //nnn

        currentWeaponUI.UpdateWeaponUI(currentWeapon);
        //resna ut det som inte behovs

        currentWeapon.removeObject();
        currentWeapon = Instantiate(newWeapon, newWeapon.transform.position, newWeapon.transform.rotation);
        currentWeapon.transform.SetParent(transform, false);
        weaponSoundEffect.clip = currentWeapon.soundEffect;
    }

    IEnumerator Cooldown(float fireRate)
    {
        waiting = true;
        canShoot = false;
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
        waiting = false;
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Powerup"))
        {
            List<Weapon> listOfSpecialWeapons = new();
            listOfSpecialWeapons.Add(sniperPrefab);
            listOfSpecialWeapons.Add(RPGPrefab);
            listOfSpecialWeapons.Add(SMGPrefab);
            

            SwapWeapon(listOfSpecialWeapons[UnityEngine.Random.Range(0, listOfSpecialWeapons.Count)]);
            
            Destroy(collision.gameObject);
        }
    }

    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Missile") || collision.gameObject.layer == LayerMask.NameToLayer("Invader"))
        {
            GameManager.Instance.OnPlayerKilled(this);
        }
    }*/
}
