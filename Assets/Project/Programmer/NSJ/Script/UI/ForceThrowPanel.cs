using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ForceThrowPanel : BaseUI
{

    TMP_Text _stackText => GetUI<TMP_Text>("StackText");
    private void Awake()
    {
        Bind();
    }

    public void UpdateText(string count)
    {
        _stackText.SetText(count);
    }
    public void UpdateText(StringBuilder count)
    {
        _stackText.SetText(count);
    }
}
