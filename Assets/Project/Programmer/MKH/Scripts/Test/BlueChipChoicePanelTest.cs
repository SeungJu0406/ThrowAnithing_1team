using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MKH
{
    public class BlueChipChoicePanelTest : MonoBehaviour
    {
        [SerializeField] public Button button;
        [SerializeField] GameObject blueChipSlotsParent;
        [SerializeField] public BlueChipSlot[] blueChipSlots;

        [SerializeField] GameObject choiceSlotsParnet;
        [SerializeField] public BlueChipSlot[] choiceSlots;

        [SerializeField] BlueChipList blueChip;

        [SerializeField] public List<AdditionalEffect> blueChipList;

        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(button.gameObject);
            RandomBlueChip();
        }

        private void Awake()
        {
            blueChipSlots = blueChipSlotsParent.GetComponentsInChildren<BlueChipSlot>();
            choiceSlots = choiceSlotsParnet.GetComponentsInChildren<BlueChipSlot>();
            blueChipList = new List<AdditionalEffect>(blueChip.blueChipList);
            Setting();
        }


        public bool AcquireEffect(AdditionalEffect effect)
        {
            for (int i = 0; i < blueChipSlots.Length; i++)
            {
                Debug.Log(i);
                if (blueChipSlots[i].Effect == null)
                {
                    blueChipSlots[i].AddEffect(effect);
                    Debug.Log(effect);
                    return true;
                }
            }
            return false;
        }

        public void Setting()
        {
            for (int i = 0; i < blueChipSlots.Length; i++)
            {
                blueChipSlots[i].SetSlot();
            }
        }

        private void Choice()
        {
            for (int i = 0; i < choiceSlots.Length; i++)
            {
                choiceSlots[i].SetSlot();
            }
        }

        public void RandomBlueChip()
        {
            List<AdditionalEffect> list = new List<AdditionalEffect>(blueChipList);
            int[] prevIndex = new int[3];
            int count = 0;
            for (int i = 0; i < choiceSlots.Length; i++)
            {
                int index = Random.Range(0, list.Count);
                AdditionalEffect effect = list[index];
                choiceSlots[i].AddEffect(effect);
                prevIndex[i] = index;
                list.RemoveAt(index);
                if (i != 0)
                {
                    if (prevIndex[i - 1] <= index)
                    {
                        count++;
                        choiceSlots[i].ListIndex = index + count;
                        prevIndex[i] = prevIndex[i - 1];
                    }
                    else
                    {
                        choiceSlots[i].ListIndex = index;
                    }
                }
                else
                {
                    choiceSlots[i].ListIndex = index;
                }
            }
        }
    }
}
