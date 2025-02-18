using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Programmer.NSJ.RND.Script.State
{
    public class BaseState
    {
        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void Exit() { }
        public virtual void OnDrawGizmos() { }
    }
}