using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Project.Programmer.NSJ.RND.Script.Test
{
    public class TestMousePointer : MonoBehaviour
    {
        [SerializeField] private bool _isPointerInvisible;

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(x => Time.timeScale == 0)
                .Subscribe(x =>
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                });
            this.UpdateAsObservable()
                .Where(x => Time.timeScale > 0)
                .Subscribe(x =>
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                });
        }
    }
}