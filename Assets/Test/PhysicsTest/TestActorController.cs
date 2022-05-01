using System;
using System.Collections.Generic;
using UnityEngine;

namespace GJKTest
{
    public class TestActorController:MonoBehaviour
    {
        public Vector3 MoveDir;
        private void Update()
        {
            int x=0;
            int y=0;
            if (Input.GetKey(KeyCode.W)) 
            {
                y += 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                y -= 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                x -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                x += 1;
            }

            MoveDir = new Vector3(x, y, 0);
            transform.position += MoveDir * Time.deltaTime * 2;
        }
    }
}
