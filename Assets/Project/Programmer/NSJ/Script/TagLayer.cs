using Assets.Project.Programmer.NSJ.RND.Script.Test.ZenjectTest;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public static partial class Tag
{
    public const string Untagged = "Untagged";
    public const string Respawn = "Respawn";
    public const string Finish = "Finish";
    public const string EditorOnly = "EditorOnly";
    public const string MainCamera = "MainCamera";
    public const string Player = "Player";
    public const string GameController = "GameController";
    public const string Monster = "Monster";
    public const string Item = "Item";
    public const string Portal = "Portal";
    public const string Trash = "Trash";
    public const string BlueChip = "BlueChip";
    public const string PortalHidden = "PortalHidden";
    public const string UnInteractable = "UnInteractable";
    public const string Canvas = "Canvas";
    public const string ObjectPool = "ObjectPool";
}
public static partial class Layer
{
    public static int EveryThing => GetLayerMaskEveryThing();
    public static int Default => LayerEnum.Default.GetLayer();  
    public static int TransparentFX => LayerEnum.TransparentFX.GetLayer();
    public static int IgnoreRaycast => LayerEnum.IgnoreRaycast.GetLayer();
    public static int Water => LayerEnum.Water.GetLayer();
    public static int UI => LayerEnum.UI.GetLayer();
    public static int Wall => LayerEnum.Wall.GetLayer();
    public static int HideWall => LayerEnum.HideWall.GetLayer();
    public static int Item => LayerEnum.Item.GetLayer();
    public static int Player => LayerEnum.Player.GetLayer();
    public static int Monster => LayerEnum.Monster.GetLayer();
    public static int ThrowObject => LayerEnum.ThrowObject.GetLayer();
    public static int Forge => LayerEnum.Forge.GetLayer();
    public static int CanPickTrash => LayerEnum.CanPickTrash.GetLayer();
    public static int CantPickTrash => LayerEnum.CantPickTrash.GetLayer();
    public static int Bullet => LayerEnum.Bullet.GetLayer();
    public enum LayerEnum
    {
        Default,
        TransparentFX ,
        IgnoreRaycast ,
        Water, 
        UI,
        Wall,
        HideWall,
        Item,
        Forge,
        Player,
        Monster,
        CanPickTrash,
        CantPickTrash,
        ThrowObject,
        Bullet
    }

    private static Dictionary<LayerEnum, int> _layerDic = new Dictionary<LayerEnum, int>();

    public static int GetLayerMaskEveryThing()
    {
        int everyThing = 0;
        everyThing |= 1 << Default;
        everyThing |= 1 << TransparentFX;
        everyThing |= 1 << IgnoreRaycast;
        everyThing |= 1 << Water;
        everyThing |= 1 << UI;
        everyThing |= 1 << Wall;
        everyThing |= 1 << HideWall;
        everyThing |= 1 << Item;
        everyThing |= 1 << Player;
        everyThing |= 1 << Monster;
        everyThing |= 1 << ThrowObject;
        everyThing |= 1 << CantPickTrash;
        everyThing |= 1 << CanPickTrash;
        everyThing |= 1 << Forge;
        everyThing |= 1 << Bullet;

        return everyThing;
    }
    private static int GetLayer(this LayerEnum layer)
    {
        if (_layerDic.ContainsKey(layer) == false)
        {
            _layerDic.Add(layer, LayerMask.NameToLayer($"{layer}"));
        }
        return _layerDic[layer];
    }


}
