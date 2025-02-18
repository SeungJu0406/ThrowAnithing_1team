using Assets.Project.Programmer.NSJ.RND.Script.ZenjectTest;
using UnityEngine;
using Zenject;

namespace Assets.Project.Programmer.NSJ.RND.Script.Test.ZenjectTest
{
    public class UntitledInstaller : MonoInstaller
    {
        [SerializeField] JenjectTester tester;
        [SerializeField] JenjectMonster monster;

        Item item;
        public override void InstallBindings()
        {
            Container
                .Bind<JenjectTester>()
                .FromInstance(tester);

            Container
                .Bind<IDamagable>()
                .To<Item>()
                .FromInstance(item);

            Container
                .BindFactory<JenjectMonster, JenjectFactory>()
                .FromComponentInNewPrefab(monster)
                .WithGameObjectName("Monster")
                .UnderTransformGroup("Monsters");
        }
    }

    public class Item : IDamagable
    {
        public void TakeDamage()
        {
            Debug.Log("¶§¸²");
        }
    }
    public interface IDamagable
    {
        void TakeDamage();
    }
}