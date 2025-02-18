using UniRx;
using UniRx.Triggers;
using UnityEngine;
namespace NSJ_TesterPanel
{
    public class TesterPanel : MonoBehaviour
    {
        [SerializeField] GameObject _canvas;

        private void Awake()
        {
            _canvas.SetActive(false);
        }

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(x => InputKey.GetButtonDown(InputKey.Cheat) == true)
                .Subscribe(x =>
                {

                    if (_canvas.activeSelf == false)
                    {
                        if (InputKey.GetActionMap() == InputType.UI)
                            return;
                        _canvas.SetActive(true);
                        InputKey.SetActionMap(InputType.UI);
                        Time.timeScale = 0f;
                    }
                });
            this.UpdateAsObservable()
              .Where(x => InputKey.PlayerInput.actions["Cancel"].WasPerformedThisFrame())
              .Subscribe(x =>
               {
                   if (_canvas.activeSelf == true)
                   {
                       _canvas.SetActive(false);
                       InputKey.SetActionMap(InputType.GAMEPLAY);
                       Time.timeScale = 1f;
                   }
               });
        }
    }
}

