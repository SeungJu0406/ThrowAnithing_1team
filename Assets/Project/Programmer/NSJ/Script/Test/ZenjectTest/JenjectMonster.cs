using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Project.Programmer.NSJ.RND.Script.ZenjectTest
{
    public class JenjectMonster : MonoBehaviour
    {
        [Inject]
        [SerializeField] JenjectTester tester;
    }

    public class JenjectFactory : PlaceholderFactory<JenjectMonster>
    {

    }
}