﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    public ColorChanger _colorChanger;

    [SerializeField] private GameObject _thruster;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _shieldVisualizer;
    [SerializeField] private GameObject _leftEngine, _rightEngine; //Similar references like this can be written like this but you hjave to be careful. This is more ofr organizational purposes.
    [SerializeField] private AudioClip _laserSoundClip;

    private AudioSource _audioSource;

    [SerializeField] private int _score;
    [SerializeField] private int _lives = 3;
    [SerializeField] private float _speed = 0;
    [SerializeField] private float _speedMultiplier = 2;
    [SerializeField] private float _fireRate = 0;
    [SerializeField] private int _ammoCount = 15;


    private float _canFire = -1f; //The -1 starts _canFire below 0. So long as Time.time is higher than _canFire, you can shoot!  
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;




    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _colorChanger = _shieldVisualizer.GetComponent<ColorChanger>();


        if (_colorChanger == null)
        {
            Debug.LogError("The _colorChanger is NULL.");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on the Player is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }

    void Update()
    {
        Boundries();
        Movement();
        Thrusters();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _ammoCount > 0)
        {
            FireLaser();
        }
    }

    void Boundries()
    {
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0); //Mathf.Clamp is used here to create a min and max value for Y as it prevents the value from going any higher or lower.

        if (transform.position.x >= 11.3)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);

        }
        else if (transform.position.x <= -11.3)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0); //Creates the Vector3 "direction" for use within this method.

        transform.Translate(direction * _speed * Time.deltaTime);
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        _ammoCount--;
        _uiManager.UpdateAmmoCount(_ammoCount);

        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _audioSource.Play();
    }



    public void Damage()
    {
        if (_isShieldActive)
        {
            _colorChanger.DamageShield();
            return;
        }

        _lives--;

        _uiManager.UpdateLives(_lives);

        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }
        else if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedPowerDownRoutine());
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);

        if (_colorChanger != null)
        {
            _colorChanger.Restoreshield();
        }
    }

    public void DeactivateShield() //Called fromt the "ColorChange" script when shield is destroyed.
    {
        _isShieldActive = false;
        _shieldVisualizer.SetActive(false);
    }

    private IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    private IEnumerator SpeedPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
        _speed /= _speedMultiplier;
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void RefillAmmo(int ammo)
    { 
        _ammoCount += ammo;
        if (_ammoCount > 15)
        {
            _ammoCount = 15;
        }
        ammo = _ammoCount;

        _uiManager.UpdateAmmoCount(ammo);
    }

    public void RestoreHealth()
    {
        _lives++;
        if (_lives > 3)
        {
            _lives = 3;
        }
        _uiManager.UpdateLives(_lives);
    }

    private void Thrusters()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !_isSpeedBoostActive)
        {
            _thruster.transform.localScale = new Vector3(0.6f, transform.localScale.y, transform.localScale.z);
            _speed *= 1.5f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _thruster.transform.localScale = new Vector3(0.4f, transform.localScale.y, transform.localScale.z);
            _speed /= 1.5f;
        }
    }
}