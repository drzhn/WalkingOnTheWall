using UnityEngine;
using System.Collections;

namespace Laser
{
    public class Laser : MonoBehaviour
    {

        private GameObject parent;
        private int index;
        void Start()
        {
            parent = transform.parent.gameObject;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PointDirectionInteract(PointDirection pd)
        {
            IndexPointDirection ipd = new IndexPointDirection(index, pd.point, pd.direction);
            parent.SendMessage("NewLaser", ipd);
        }
        public void SetIndex(int value)
        {
            index = value;
        }
    }
}