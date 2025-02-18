using MKH;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotButton : BaseUI
{
    [SerializeField] bool _isFirstSelect;
    [SerializeField] int _index;

    public GameObject Outline;
    public InventoryController _controller;
    public InventorySlot _slot;
    public Button _button;

    private void Awake()
    {
        Bind();
        _controller = GetComponentInParent<InventoryController>();
        _slot = GetComponentInParent<InventorySlot>();
        Outline = GetUI("Outline");
        _button = GetComponent<Button>();
    }
    //private void OnEnable()
    //{
    //    if (_isFirstSelect == true)
    //    {
    //        EventSystem.current.SetSelectedGameObject(gameObject);
    //    }
    //}
    private void Update()
    {
    }
    //public void OnSelect(BaseEventData eventData)
    //{
    //    _controller.ChangeSelectButton(_slot);
    //}
}
