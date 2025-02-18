using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDebuff
{
    int MaxHp { get; set; }
    int CurHp { get; set; }
    float MoveSpeed {  get; set; }
    float JumpPower { get; set; }
    float AttackSpeed {  get; set; }
}