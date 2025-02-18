using TMPro;
using UnityEngine;

public class TestBlueChip : MonoBehaviour
{
    [SerializeField] public AdditionalEffect Effect;
    [SerializeField] TMP_Text nameText;

    private void Start()
    {
        nameText.SetText(Effect.Name.GetText());
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 180, 0);
    }

    public void SetBlueChip(AdditionalEffect additional)
    {
        Effect = additional;
        nameText.SetText(Effect.Name.GetText());
    }


}
