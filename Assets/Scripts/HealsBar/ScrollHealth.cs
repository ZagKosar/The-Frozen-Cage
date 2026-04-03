using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollHealth : MonoBehaviour
{
    Sprite h_Status;
    Image m_Image;
    [SerializeField] float _scrollSpeed;
    [SerializeField] Sprite[] _healthStatus;
    [SerializeField] GameObject _status;

    private void Start()
    {
        _status.GetComponent<Image>().sprite = h_Status;
        m_Image = this.GetComponent<Image>();
        m_Image.color = new Color32(0, 255, 0, 255);
        h_Status = _healthStatus[0];
    }

    private void Update()
    {
        _status.GetComponent<Image>().sprite = h_Status;

        m_Image.material.mainTextureOffset = m_Image.material.mainTextureOffset + new Vector2(Time.deltaTime * (-_scrollSpeed / 10), 0f);

        if (false)
        {
            m_Image.color = new Color32(0, 255, 0, 255);
            h_Status = _healthStatus[0];
        }
        if (false)
        {
            m_Image.color = new Color32(255, 255, 0, 255);
            h_Status = _healthStatus[1];
        }
        if (false)
        {
            m_Image.color = new Color32(255, 0, 0, 255);
            h_Status = _healthStatus[2];
        }
    }
}
