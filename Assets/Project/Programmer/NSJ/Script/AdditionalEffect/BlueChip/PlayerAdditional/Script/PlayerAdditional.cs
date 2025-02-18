using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAdditional : AdditionalEffect
{
    protected PlayerController Player;
    protected Transform transform => Player.transform;
    protected Rigidbody Rb => Player.Rb;
    protected PlayerModel Model => Player.Model;
    protected BattleSystem Battle => Player.Battle;
    protected PlayerController.State CurState => Player.CurState;

    public void Init(PlayerController player, AdditionalEffect addtional)
    {
        Origin = addtional.Origin;
        Player = player;
    }

    protected int GetPlayerAttackPower(int attackPower)
    {
        return (Model.AttackPower - (int)Model.Data.EquipStatus.Damage) + attackPower;
    }
}
