/*using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PausePanel : MonoBehaviour
{
    public enum Bundle { None ,Pause, Option, Size }
    [SerializeField] Main_Option _pause;
    [SerializeField] Main_Option _option;

    private Main_Option[] _bundles = new Main_Option[(int)Bundle.Size];


    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        ChangeBundle(PausePanel.Bundle.None);
    }

    private void Update()
    {
        if (InputKey.GetButtonDown(InputKey.Cancel))
        {
            PauseOnOff();
        }
    }

    void PauseOnOff()
    {
        if(Time.timeScale == 0)
        {
            ChangeBundle(PausePanel.Bundle.None);
        }
        else
        {
            ChangeBundle(PausePanel.Bundle.Pause);
        }
      
    }
    public void ChangeBundle(Bundle bundle)
    {
        if (bundle == Bundle.None)
        {         
            for (int i = 1; i < _bundles.Length; i++)
            {
                _bundles[i].gameObject.SetActive(false);
            }
            Time.timeScale = 1;
        }
        else
        {        
            for (int i = 1; i < _bundles.Length; i++)
            {
                if (i == (int)bundle)
                {
                    _bundles[i].gameObject.SetActive(true);
                }
                else
                {
                    _bundles[i].gameObject.SetActive(false);
                }
            }
            Time.timeScale = 0;
        }
    }

    private void Init()
    {
        _bundles[(int)Bundle.Pause] = _pause;
        _bundles[(int)Bundle.Option] = _option;

        for (int i = 1; i < _bundles.Length; i++)
        {
            _bundles[i].Panel = this;
        }
    }
}
*/