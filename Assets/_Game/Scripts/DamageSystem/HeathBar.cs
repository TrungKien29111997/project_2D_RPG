using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeathBar : MonoBehaviour
{
    [SerializeField] Image image;
    Character _character;
    GameObject _camera;
    float maxHP;

    public void Start()
    {
        OnInit();
    }

    public void Update()
    {
        if (maxHP == 0) maxHP = _character.hp;
        transform.forward = _camera.transform.forward;
        image.fillAmount = Mathf.Lerp(image.fillAmount, _character.hp / maxHP, Time.deltaTime * 2f);
    }

    public void OnInit()
    {
        image.fillAmount = 1;
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        _character = GetComponentInParent<Character>();
        if (_character.gameObject.CompareTag("Player"))
        {
            image.color = Color.green;
        }
        else if (_character.gameObject.CompareTag("Enemy"))
        {
            image.color = Color.red;
        }
    }

    public void OnDespawn()
    {
        
    }
}
