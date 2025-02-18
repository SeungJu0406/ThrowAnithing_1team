using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MKH
{
    public class SampleScript : MonoBehaviour
    {
        [SerializeField] private InventoryMain mInventoryMain;

        [SerializeField] private Item mHelmetItem, mShirtsItem;

        private void OnGUI()
        {
           if(GUI.Button(new Rect(20, 20, 300, 40), "Çï¸ä"))
            {
                mInventoryMain.AcquireItem(mHelmetItem);
            }

            if (GUI.Button(new Rect(400, 20, 300, 40), "¼ÅÃ÷"))
            {
                mInventoryMain.AcquireItem(mShirtsItem);
            }
        }
    }
}
