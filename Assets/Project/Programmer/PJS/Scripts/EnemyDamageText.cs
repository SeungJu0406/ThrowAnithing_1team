using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyDamageText : MonoBehaviour
{
    private TMP_Text damageText;
    private Color arpa;
    private int damage;

    public int Damage { set { damage = value; } }

    private void Start()
    {
        damageText = GetComponent<TMP_Text>();
        damageText.SetText(damage.ToString());
        StartCoroutine(TextViewRoutine());
    }

    IEnumerator TextViewRoutine()
    {
        arpa = damageText.color;
        
        while (true)
        {
            if(arpa.a <= 0.1f)
            {
                Destroy(gameObject);
                yield break;
            }

            transform.Translate(Vector3.up * 1f * Time.deltaTime);
            arpa.a = Mathf.Lerp(damageText.color.a, 0, Time.deltaTime * 1.5f);
            damageText.color = arpa;
            yield return null;
        }
    }
}
