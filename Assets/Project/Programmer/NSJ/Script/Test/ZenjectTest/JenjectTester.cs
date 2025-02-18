using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Project.Programmer.NSJ.RND.Script.ZenjectTest
{
    public class JenjectTester : MonoBehaviour
    {
        [Inject]
        [SerializeField] JenjectFactory factory;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                factory.Create();
            }
           
        }
    }
}