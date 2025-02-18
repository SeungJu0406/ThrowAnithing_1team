using UnityEngine;
using Zenject;

namespace Assets.Project.Programmer.NSJ.RND.Script.ZenjectTest
{
    public class UntitledInstaller : MonoInstaller
    {
        [SerializeField] private JenjectTester jenjectTester;
        public override void InstallBindings()
        {
            Container.Bind<JenjectTester>()
                .FromInstance(jenjectTester);
        }
    }
}