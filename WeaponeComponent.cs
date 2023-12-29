using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponeComponent : MonoBehaviour
{
    [Header("WeaponeSetting")]
    public float damage = 30f;
    public float range = 100f;
    public float rateOfFire = 30f;
    private float nextTimeToFire = 0f;
    public Camera fpsCam;
    public Animator animator;
    private bool isFiring = false;

    [Header("Ammo")]
    
    public Int32 currentAmmo = 30;
    public Int32 totalAmmo = 900;
    public Int32 maxClipSize = 30;
    private Int32 tempAmmo = 0;
    private bool isReloading = false;

    [Header("Audio")]
    AudioSource audioSource;
    public AudioClip audioClip;

    [Header("UI")]
    public Text ammoText;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            if(currentAmmo>0)
            {
                if(isReloading)
                {
                    
                }
                else
                {
                    currentAmmo--;
                    isFiring = true;
                    nextTimeToFire = Time.time+1f/rateOfFire;
                    animator.SetBool("isFiring", true);
                    audioSource.PlayOneShot(audioClip);
                    Shoot();
                }
            }
        }    
        else
        {
            isFiring = false;
            animator.SetBool("isFiring", false);
        }

        Debug.Log(currentAmmo + "CurrentAmmo");
        Debug.Log(maxClipSize + "Max");


        if(Input.GetKey(KeyCode.R))
        {
            StartCoroutine(Reload());
        }

        ammoText.text = currentAmmo.ToString();

    }

    void Shoot()
    {

        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward,out hit, range))
        {
            Debug.DrawLine(fpsCam.transform.position, hit.point, Color.red, 0.2f);
        }
    }

    void Relaoding()
    {
       
    }

    IEnumerator Reload()
    {
        Int32 pickB = currentAmmo+totalAmmo;
        bool pickASelect = pickB>= maxClipSize;

        Int32 SelectCurrent = (currentAmmo+totalAmmo>=maxClipSize) ? maxClipSize : currentAmmo+totalAmmo;
        Int32 clampAmmo = Mathf.Clamp(totalAmmo-maxClipSize-tempAmmo, 0, totalAmmo);
        Int32 selectAmmo = (pickASelect) ? clampAmmo : 0;

        if(currentAmmo == maxClipSize || totalAmmo<=0)
        {

        }
        else
        {
            isReloading = true;
            tempAmmo = currentAmmo;
            currentAmmo = SelectCurrent;
            totalAmmo = selectAmmo;
            animator.SetBool("isReloading", true);
            isReloading = false;
            yield return new WaitForSeconds(2);
            animator.SetBool("isReloading", false);
        }
    }
}
