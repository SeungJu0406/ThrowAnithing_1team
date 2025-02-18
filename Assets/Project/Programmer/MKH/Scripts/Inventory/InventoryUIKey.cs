using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIKey : BaseUI
{
    const string PC = "PC";
    const string CONSOLE = "Console";

    private GameObject _pc;
    private GameObject _console;

    private void Awake()
    {
        Bind();
        _pc = GetUI("PCKey");
        _console = GetUI("ConsoleKey");
    }

    private void Update()
    {
        _pc.SetActive(InputKey.PlayerInput.currentControlScheme == PC);
        _console.SetActive(InputKey.PlayerInput.currentControlScheme == CONSOLE);
    }
}
