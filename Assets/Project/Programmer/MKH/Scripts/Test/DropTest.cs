using System.Collections;
using UnityEngine;

namespace MKH
{
    public class DropTest : MonoBehaviour
    {
        public DropList dropList;
        private GameObject dropPrefab;
        public GameObject particle;
        public GameObject particle1;
        public DropItemTable dropTable;
        private GameObject obj;
        public GameObject blueChip;

        private void Start()
        {
            dropPrefab = GetComponent<GameObject>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                StartCoroutine(Drop());
            }

            if(Input.GetKeyDown(KeyCode.F2))
            {
                BlueChip();
            }
        }

        private void DropItem()
        {
            //dropPrefab = dropList.itemList[Random.Range(0, dropList.Count)];
            //GameObject obj = Instantiate(dropPrefab, transform.position, Quaternion.identity);
            //Destroy(obj, 10f);

             dropTable.DropListTable1(transform.position, Quaternion.identity);


            //GameObject obj = dropPrefab;
            //obj.transform.position = transform.position;
            //obj.transform.rotation = Quaternion.identity;
        }

        private void Pat()
        {
            GameObject particles = Instantiate(particle, transform.position, Quaternion.Euler(-90f, 0, 0));
            Destroy(particles, 0.5f);
        }

        private void End()
        {
            GameObject particles = Instantiate(particle1, transform.position, Quaternion.Euler(-90f, 0, 0));
            Destroy(particles, 1f);
        }

        IEnumerator Drop()
        {
            Pat();
            yield return new WaitForSeconds(0.2f);
            DropItem();
            yield return new WaitForSeconds(10f);
            End();

        }

        private void BlueChip()
        {
            Instantiate(blueChip, transform.position, Quaternion.identity);
        }
    }

}
