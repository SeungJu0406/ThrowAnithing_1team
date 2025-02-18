using MKH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BlueChipChoice : MonoBehaviour
{
    [Inject]
    private PlayerData playerData;

    [SerializeField] GameObject choice;
    [SerializeField] GameObject choicePanel;

    private TestBlueChip blueChip;

    [SerializeField] BlueChipPanel blueChipPanel;
    PlayerController _player;

    [SerializeField] AudioClip OpenClip;

    [SerializeField] GameObject pcText;
    [SerializeField] GameObject consoleText;

    private void Awake()
    {
        _player = GetComponent<PlayerController>();
        blueChipPanel = playerData.Inventory.BlueChipPanel;
        choice = playerData.Inventory.BlueChipChoice;
        choicePanel = playerData.Inventory.ChoicePanel;
        pcText = playerData.Inventory.PcText;
        consoleText = playerData.Inventory.ConsloeText;
    }
    private void Start()
    {       
        choice.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Tag.BlueChip)
        {
            GameObject _pc = pcText;
            GameObject _console = consoleText;
            // 각 디바이스에 맞는 텍스트 활성화
            _pc.SetActive(InputKey.PlayerInput.currentControlScheme == InputType.PC);
            _console.SetActive(InputKey.PlayerInput.currentControlScheme == InputType.CONSOLE);

            choice.SetActive(true);
            if(_addBlueChipRoutine == null)
            {
                _addBlueChipRoutine = StartCoroutine(AddBlueChipRoutine(other));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == Tag.BlueChip)
        {
            choice.SetActive(false);
            if(_addBlueChipRoutine != null)
            {
                StopCoroutine(_addBlueChipRoutine);
                _addBlueChipRoutine = null;
            }
        }
    }

    Coroutine _addBlueChipRoutine;
    IEnumerator AddBlueChipRoutine(Collider other)
    {
        blueChip = other.transform.GetComponent<TestBlueChip>();
        while (true)
        {
            if (other.gameObject.tag == Tag.BlueChip)
            {
                if (InputKey.PlayerInput.actions["Interaction"].WasPressedThisFrame())
                {
                    SoundManager.PlaySFX(OpenClip);
                    choice.SetActive(false);
                    choicePanel.SetActive(true);
                    InputKey.SetActionMap(InputType.UI);

                    _addBlueChipRoutine = null;
                    Destroy(other.gameObject);
                    yield break;
                }
            }
            yield return null;
        }
    }
}
