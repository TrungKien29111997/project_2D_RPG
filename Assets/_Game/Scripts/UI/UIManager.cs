using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] GameObject heathBar;
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    public void SetCoin(int coin)
    {
        textMesh.text = coin.ToString();
    }

    public void SpawnHealthBar(Transform parent, float offset)
    {
        Instantiate(heathBar, parent.position + new Vector3(0, offset, 0), Quaternion.identity, parent);
    }
}
