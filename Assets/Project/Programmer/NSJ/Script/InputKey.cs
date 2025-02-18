using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputKey : MonoBehaviour
{
    public static InputKey Instance;
    #region 인풋시스템
    public enum Action
    {
        Move, CameraMove, MouseDelta, // Axis
        Jump, Throw, Special, Melee, LoakOn, LoakOff, Dash, Interaction, Drain, OpenSettings, InvenOpen, Cheat, //GamePlay
        Choice, Cancel, Break, InvenClose
    } // None
    public enum Axis { None, Axis }
 
    public struct InputStruct
    {
        public Action action;
        public Axis Axis;
        public Vector2 Vector;
        public string Name;
    }
    // GamePlay
    public static InputStruct Move;
    public static InputStruct CameraMove;
    public static InputStruct MouseDelta;
    public static InputStruct Jump;
    public static InputStruct Throw;
    public static InputStruct Special;
    public static InputStruct Melee;
    public static InputStruct LoakOn;
    public static InputStruct LoakOff;
    public static InputStruct Dash;
    public static InputStruct Interaction;
    public static InputStruct Drain;
    public static InputStruct OpenSetting;
    public static InputStruct InvenOpen;
    public static InputStruct Cheat;
    // UI
    public static InputStruct InvenOpenUI;
    public static InputStruct CancelUI;
    public static InputStruct Choice;
    public static InputStruct Break;
    public static InputStruct InvenClose;

    private Dictionary<string, InputStruct> m_inputStructDic = new Dictionary<string, InputStruct>();
    private static Dictionary<string, InputStruct> _inputStructDic { get { return Instance.m_inputStructDic; } }
    private  List<InputStruct> m_inputStructs = new List<InputStruct>();
    private static List<InputStruct> _inputStructs { get { return Instance.m_inputStructs; } }
    private PlayerInput m_playerInput;
    public static PlayerInput PlayerInput { get { return Instance.m_playerInput; } }

    private void Awake()
    {
        if(InitSingleTon() ==false) return;
        Init();

        m_playerInput = GetComponent<PlayerInput>();
    }
    private bool InitSingleTon()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            return true;
        }
        else
        {
            Destroy(gameObject);
            return false;
        }
    }

    private void Init()
    {
        Move = GetInputStruct(Action.Move, Axis.Axis,"Move");
        CameraMove = GetInputStruct(Action.CameraMove, Axis.Axis, "CameraMove");
        MouseDelta = GetInputStruct(Action.MouseDelta, Axis.Axis, "MouseDelta");
        Jump = GetInputStruct(Action.Jump, Axis.None, "Jump");
        Throw = GetInputStruct(Action.Throw, Axis.None, "Ranged_Attack");
        Special = GetInputStruct(Action.Special, Axis.None, "Special_Attack");
        Melee = GetInputStruct(Action.Melee, Axis.None, "Melee_Attack");
        LoakOn = GetInputStruct(Action.LoakOn, Axis.None, "Loak_On");
        LoakOff = GetInputStruct(Action.LoakOff, Axis.None,"Loak_Off");
        Dash = GetInputStruct(Action.Dash, Axis.None, "Dash");
        Interaction = GetInputStruct(Action.Interaction, Axis.None, "Interaction");
        Drain = GetInputStruct(Action.Drain, Axis.None, "Drain");
        OpenSetting = GetInputStruct(Action.OpenSettings, Axis.None, "Open_Setting");
        InvenOpen = GetInputStruct(Action.InvenOpen, Axis.None, "InvenOpen");
        Cheat = GetInputStruct(Action.Cheat, Axis.None,"Cheat");
        CancelUI = GetInputStruct(Action.Cancel, Axis.None, "Cancel");
        Break = GetInputStruct(Action.Break, Axis.None, "Break");
        InvenClose = GetInputStruct(Action.InvenClose, Axis.None, "InvenClose");
    }


    private void LateUpdate()
    {
    }

    //public static bool GetButton(InputStruct inputStruct)
    //{
    //    if (_inputStructDic[inputStruct.Name].Axis == Axis.Axis)
    //        return false;

    //    if (_inputStructDic[inputStruct.Name].CurPress == true)
    //        return true;
    //    else
    //        return false;
    //}
    public static bool GetButtonDown(InputStruct inputStruct)
    {
        if (_inputStructDic[inputStruct.Name].Axis == Axis.Axis)
            return false;

        if (PlayerInput.actions[inputStruct.Name].WasPerformedThisFrame())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool GetButtonUp(InputStruct inputStruct)
    {
        if (_inputStructDic[inputStruct.Name].Axis == Axis.Axis)
            return false;

        if (PlayerInput.actions[inputStruct.Name].WasReleasedThisFrame())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static Vector2 GetAxis(InputStruct inputStruct)
    {
        if (_inputStructDic[inputStruct.Name].Axis == Axis.None)
            return default;

        return _inputStructDic[inputStruct.Name].Vector;
    }


    public static void SetActionMap(string actionMap)
    {
        PlayerInput.SwitchCurrentActionMap(actionMap);
    }
    public static string GetActionMap()
    {
        return PlayerInput.currentActionMap.name;
    }
    private static void SetVector(InputStruct inputStruct, Vector2 value)
    {
        InputStruct newInputStruct = _inputStructDic[inputStruct.Name];
        newInputStruct.Vector = value;
        _inputStructDic[inputStruct.Name] = newInputStruct;
    }
    private static InputStruct GetInputStruct(Action action, Axis axis, string name)
    {
        InputStruct inputStruct = new InputStruct();
        inputStruct.Name = name;
        inputStruct.Axis = axis;
        inputStruct.action = action;
        _inputStructDic.Add(name, inputStruct);
        _inputStructs.Add(inputStruct);
        return inputStruct;
    }
    private void ProcessInput(InputValue value, InputStruct input)
    {
        if (input.Axis == Axis.Axis)
        {
            Vector2 vector = value.Get<Vector2>();
            SetVector(input, vector);
        }
    }

    private void OnMove(InputValue value)
    {
        ProcessInput(value, Move);
    }
    private void OnCameraMove(InputValue value)
    {
        ProcessInput(value, CameraMove);
    }
    private void OnMouseDelta(InputValue value)
    {
        ProcessInput(value, MouseDelta);
    }
//    private void OnJump(InputValue value)
//    {
//;        ProcessInput(value, Jump);
//    }
//    private void OnRanged_Attack(InputValue value)
//    {
//        ProcessInput(value, Throw);
//    }
//    private void OnSpecial_Attack(InputValue value)
//    {
//        ProcessInput(value, Special);
//    }
//    private void OnMelee_Attack(InputValue value)
//    {
//        ProcessInput(value, Melee);
//    }

//    private void OnLoak_On(InputValue value)
//    {
//        ProcessInput(value, LoakOn);
//    }
//    private void OnLoak_Off(InputValue value)
//    {
//        ProcessInput(value, LoakOff);
//    }
//    private void OnDash(InputValue value)
//    {
//        ProcessInput(value, Dash);
//    }
//    private void OnInteraction(InputValue value)
//    {
//        ProcessInput(value, Interaction);
//    }
//    private void OnDrain(InputValue value)
//    {
//        ProcessInput(value, Drain);
//    }
//    private void OnOpen_Settine(InputValue value)
//    {
//        ProcessInput(value, OpenSetting);
//    }

//    private void OnInvenOpen(InputValue value)
//    {
//        ProcessInput(value, InvenOpen);
//    }
//    private void OnCheat(InputValue value)
//    {
      
//        ProcessInput(value, Cheat);
//    }
    #endregion
}
