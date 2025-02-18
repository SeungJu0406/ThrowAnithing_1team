using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ThrowReduce", menuName = "AdditionalEffect/PrevThrow/ThrowReduce")]
public class ThrowReduce : ThrowAdditional
{
    [Range(0,100)]
    [SerializeField] float _probability;
    public override void Enter()
    {
        // 투척물이 소모되지 않음
        if (_throwObject.Data.ID == 0)
            return;

        if (Random.Range(0, 100) <= _probability)
        {
            Player.AddThrowObject(_throwObject);
        }
    }
}
