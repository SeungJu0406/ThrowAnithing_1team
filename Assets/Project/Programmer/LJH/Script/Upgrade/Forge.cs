using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Forge : MonoBehaviour
{
    [SerializeField] GameObject upPopup;
    [SerializeField] GameObject pcPopup;
    [SerializeField] GameObject padPopup;

    [SerializeField] GameObject _ui;

    public bool IsUIActive;
    public bool IsActive;
    PlayerController _player;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag(Tag.Player).GetComponent<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Tag.Player))
        {
            SoundManager.PlaySFX(SoundManager.Data.UI.PopUpOn);
            upPopup.SetActive(true);
            pcPopup.SetActive(InputKey.PlayerInput.currentControlScheme == InputType.PC);
            padPopup.SetActive(InputKey.PlayerInput.currentControlScheme == InputType.CONSOLE);
            IsActive = true;
        }

    }

        private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(Tag.Player))
        {
            SoundManager.PlaySFX(SoundManager.Data.UI.PopUpOff);
            pcPopup.SetActive(false);
            padPopup.SetActive(false);
            upPopup.SetActive(false);
            _ui.SetActive(false);
            IsActive = false;
        }
    }

    private void Update()
    {
        if (IsActive == false)
        {
            return;
        }

        if (InputKey.GetButtonDown(InputKey.CancelUI))
        {
            SetUnActiveUI();
        }

        //UI 활성화 됬는지 감지
        if (_ui.activeSelf == true)
        {
            IsUIActive = true;
        }
        else
        {
            IsUIActive = false;
        }
    }

    public void SetUnActiveUI()
    {
      
        if (_ui.activeSelf == true && IsUIActive == true)
        {
            _ui.SetActive(false);
        }
    }
}


