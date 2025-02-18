using MKH;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlueChipChoiceControlTest : MonoBehaviour
{
    [SerializeField] Button[] removeButtons;
    [SerializeField] GameObject popUp;
    [SerializeField] GameObject errorPopUp;
    [SerializeField] BlueChipPanel blueChipPanel;
    [SerializeField] BlueChipChoicePanelTest blueChipChoicePanel;
    [SerializeField] int popUpChoice;

    [SerializeField] BlueChipList blueChipList;


    public void Canecel()
    {
        gameObject.SetActive(false);
    }

    public void Choice(int number)
    {
        for (int i = 0; i < blueChipPanel.mSlots.Length; i++)
        {
            if (blueChipPanel.mSlots[i].Effect == null)
            {
                blueChipPanel.mSlots[i].AddEffect(blueChipChoicePanel.choiceSlots[number].Effect);
                blueChipChoicePanel.blueChipSlots[i].AddEffect(blueChipChoicePanel.choiceSlots[number].Effect);
                blueChipChoicePanel.blueChipList.RemoveAt(blueChipChoicePanel.choiceSlots[number].ListIndex);
                Debug.Log(blueChipChoicePanel.choiceSlots[number].ListIndex);
                gameObject.SetActive(false);

                return;
            }
            else if (blueChipPanel.mSlots[0].Effect != null 
                && blueChipPanel.mSlots[1].Effect != null 
                && blueChipPanel.mSlots[2].Effect != null 
                && blueChipPanel.mSlots[3].Effect != null)
            {
                ErrorPopUp();
            }
        }
    }

    public void PopUp(int number)
    {
        popUp.SetActive(true);
        popUpChoice = number;
    }

    public void ErrorPopUp()
    {
        StartCoroutine(Error());
    }

    public void Remove()
    {
        blueChipChoicePanel.blueChipList.Add(blueChipPanel.mSlots[popUpChoice].Effect);
        blueChipPanel.mSlots[popUpChoice].ClearSlot();
        blueChipChoicePanel.blueChipSlots[popUpChoice].ClearSlot();
        popUp.SetActive(false);
        EventSystem.current.SetSelectedGameObject(blueChipChoicePanel.button.gameObject);
    }

    
    

    public void ClosePopUp()
    {
        popUp.SetActive(false);
        EventSystem.current.SetSelectedGameObject(blueChipChoicePanel.button.gameObject);
    }

    IEnumerator Error()
    {
        errorPopUp.SetActive(true);
        yield return 1f.GetDelay();
        errorPopUp.SetActive(false);
    }
}
