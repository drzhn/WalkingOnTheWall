using UnityEngine;
using MathFunctions;
using System.Collections;
using System.Collections.Generic;

namespace Laser
{
    public class LaserSource : MonoBehaviour
    {

        public GameObject laserPrefab;
        public GameObject flarePrefab;

        private MathFunctions.MathFunctions math = new MathFunctions.MathFunctions();
        private List<GameObject> laserList = new List<GameObject>();
        private List<Vector3> positionList = new List<Vector3>();
        private GameObject flare;
        void Start()
        {
            CreateLaser(transform.position, -transform.up);
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void NewLaser(IndexPointDirection ipd)
        {
            //Debug.Log("Source " + ipd.index.ToString() + ipd.point.ToString() + ipd.direction.ToString());
            Vector3 beginPoint = positionList[ipd.index];
            for (int i = laserList.Count; i > ipd.index; i--)
            {
                Destroy(laserList[i-1]);
                laserList.RemoveAt(i-1);
                positionList.RemoveAt(i-1);
            }
            InstantiateLaser(beginPoint, ipd.point);
            CreateLaser(ipd.point, ipd.direction);
        }
        void CreateLaser(Vector3 origin, Vector3 direction)
        {
            Ray ray = new Ray(origin, direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                InstantiateLaser(origin, hit.point);
            }
            else
            {
                //create 1000-meter ray
                Vector3 point = direction.normalized * 1000 + origin;
                InstantiateLaser(origin, point);
            }
        }

        private void InstantiateLaser(Vector3 origin, Vector3 end)
        {
            Vector3 direction = end - origin;
            Vector3 position = (origin + end) / 2;
            Quaternion rotation = Quaternion.LookRotation(math.RandomNormal(direction), direction);
            Vector3 scale = new Vector3(0.05f, Vector3.Distance(end, origin) / 2, 0.05f);
            GameObject __laser = (GameObject)Instantiate(laserPrefab, position, rotation);
            __laser.transform.localScale = scale;
            __laser.transform.parent = gameObject.transform;
            laserList.Add(__laser);
            positionList.Add(origin);
            //maxIndex += 1;
            laserList[laserList.Count - 1].SendMessage("SetIndex", laserList.Count - 1);
            if (flare != null) Destroy(flare);
            flare = (GameObject)Instantiate(flarePrefab, end, Quaternion.identity);
            flare.transform.parent = gameObject.transform;
        }
    }
    public class IndexPointDirection
    {
        public int index;
        public Vector3 point;
        public Vector3 direction;
        public IndexPointDirection(int _index, Vector3 _point, Vector3 _direction)
        {
            index = _index;
            point = _point;
            direction = _direction;
        }
    }
    public class PointDirection
    {
        public Vector3 point;
        public Vector3 direction;
        public PointDirection(Vector3 _point, Vector3 _direction)
        {
            point = _point;
            direction = _direction;
        }
    }
}