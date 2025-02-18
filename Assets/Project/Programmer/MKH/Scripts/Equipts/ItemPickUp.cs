using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MKH
{
    public class ItemPickUp : MonoBehaviour
    {
        [Header("해당 오브젝트에 할당되는 아이템")]
        [SerializeField] private Item mItem;
        public Item Item { get { return mItem; } }

        Vector3 pos = new Vector3(0, 1, 0);
        private float range = 0.2f;
        private float speed = 2f;

        private void Start()
        {
            pos = transform.position;
        }

        private void Update()
        {
            Vector3 v = pos;
            v.x = transform.position.x;
            v.y += range * Mathf.Sin(Time.time * speed);
            v.z = transform.position.z;
            transform.position = v;

            transform.LookAt(Camera.main.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 180, 0);
        }

    }
}
