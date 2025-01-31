﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    Player player;

    [SerializeField] private float _speed = 0;
    [SerializeField] private int _powerupID;

    [SerializeField] private AudioClip _audioClip;

    [SerializeField] private Transform target;

    private int _ammoRefill = 15;
    private bool _isBeingAttractedToPlayer = false;
    private int _commonPointValue = 10;
    private int _uncommonPointValue = 20;
    private int _rarePointValue = 40;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        target = player.transform;
    }

    void Update()
    {
        if (!_isBeingAttractedToPlayer)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);

            if (transform.position.y < -4.5f)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            if (target != null)
            {
                transform.Translate(Vector3.right * (_speed * 2) * Time.deltaTime);
                transform.right = target.position - transform.position;
            }
        }
    }

    public void AttractPowerup()
    {
        _isBeingAttractedToPlayer = true;
    }

    public void StopAttractingPowerup()
    {
        _isBeingAttractedToPlayer = false;
        transform.localEulerAngles = new Vector3(0, 0, 0);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            AudioSource.PlayClipAtPoint(_audioClip, transform.position);

            if (player != null)
            {
                switch(_powerupID)    
                {
                    case 0:
                        player.TripleShotActive(); //Common Spawn
                        player.AddScore(_commonPointValue);
                        break;
                    case 1:
                        player.SpeedBoostActive(); //Common Spawn
                        player.AddScore(_commonPointValue);
                        break;
                    case 2:
                        player.RefillAmmo(_ammoRefill); //Common Spawn
                        player.AddScore(_commonPointValue);
                        break;
                 /////////////////////////////////////////////////////////////////////////
                    case 3:
                        player.ShieldActive(); //Uncommon Spawn
                        player.AddScore(_uncommonPointValue);
                        break; 
                    case 4:
                        player.RestoreHealth(); //Uncommon Spawn
                        player.AddScore(_uncommonPointValue);
                        break;
                    case 5:
                        player.NegaShroomActive(); //Uncommon Spawn
                        player.AddScore(_uncommonPointValue);
                        break;
                 ///////////////////////////////////////////////////////////////////////
                    case 6:
                        player.SuperBeamActive(); //Rare Spawn
                        player.AddScore(_rarePointValue);
                        break;
                    case 7:
                        player.SuperMissileActive(); //Rare Spawn
                        player.AddScore(_rarePointValue);
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;
                }
            }
            Destroy(this.gameObject);
        }
    }
}
