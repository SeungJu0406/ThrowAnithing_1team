using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.DefaultInputActions;

public class NewContinue : BaseUI
{
    MainSceneBinding binding;
    PlayerActions input;
    PlayerInput playerInput;

    Button slot1;
    Button slot2;
    Button slot3;

    List<Button> buttons = new List<Button>();

    private void Awake()
    {
        Bind();
        Init();
    }
    private void OnEnable()
    {
        input.Enable();
        input.UI.Cancel.performed += OnCancel;

        

    }

    private void OnDisable()
    {
        input.UI.Cancel.performed -= OnCancel;
        input.Disable();
    }
    void Start()
    {
    }

    private void Update()
    {
        binding.ButtonFirstSelect(slot1.gameObject);
        SelectedSlotHighlight(buttons);

        if (playerInput.actions["UIMove"].WasPressedThisFrame())
        {
            if (playerInput.actions["UIMove"].ReadValue<Vector2>().y != 0)
                SoundManager.PlaySFX(SoundManager.Data.UI.NaviMove);
        }

        if (playerInput.actions["LeftClick"].WasPressedThisFrame())
        {
            SoundManager.PlaySFX(SoundManager.Data.UI.ClickNull);
        }
    }

    /// <summary>
    /// 선택된 슬롯 색상 변경
    /// </summary>
    /// <param name ="slots">지금 쓰는 버튼 리스트</param>
    public void SelectedSlotHighlight(List<Button> slots)
    {
        if (EventSystem.current.currentSelectedGameObject != null)
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].GetComponent<Image>().color = Color.white;

                if (slots[i] == EventSystem.current.currentSelectedGameObject.GetComponent<Image>())
                    slots[i].GetComponent<Image>().color = new Color(1, 0.5f, 0, 1);
            }
    }


    void OnCancel(InputAction.CallbackContext context)
    {
        Debug.Log("취소실행");
        Cancel();
    }

    void Cancel()
    {
        binding.CanvasChange(binding.mainCanvas.gameObject, gameObject);
    }


    void Init()
    {
        playerInput = InputKey.PlayerInput;
        binding = GetComponentInParent<MainSceneBinding>();
        input = new PlayerActions();

        buttons.Add(slot1 = GetUI<Button>("ContinueSlot1"));
        buttons.Add(slot2 = GetUI<Button>("ContinueSlot2"));
        buttons.Add(slot3 = GetUI<Button>("ContinueSlot3"));

    }
}
