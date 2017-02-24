using UnityEngine;
using MathFunctions;
using Laser;
using System.Collections;

public class ObjectInteraction : MonoBehaviour
{

    public GameObject pointer_prefab;

    private GameObject _pointer;
    private bool isInteraction = false;
    private Vector3 pointInteract;
    private Vector3 directionInteract;
    private float distanceInteract;
    private GameObject objectInteract;
    private Rigidbody rigidbodyInteract;
    private string tagInteract;
    private MathFunctions.MathFunctions math = new MathFunctions.MathFunctions();
    //private bool isMovable = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Laser")
                {
                    _pointer = (GameObject)Instantiate(pointer_prefab, hit.point, Quaternion.identity);
                    isInteraction = true;
                    pointInteract = hit.point;
                    distanceInteract = hit.distance;
                    objectInteract = hit.collider.gameObject;
                    tagInteract = hit.collider.tag;
                    //isMovable = true;

                    //Debug.Log(pointInteract);
                    //Debug.Log(hit.collider.transform.InverseTransformPoint(pointInteract));
                    //Debug.Log(hit.collider.transform.TransformPoint(hit.collider.transform.InverseTransformPoint(pointInteract)));
                }
                else
                    isInteraction = false;
                //isMovable = false;
            }
            else
                isInteraction = false;
            //isMovable = false;
        }
        if (isInteraction /* && isMovable*/)
        {
            if (tagInteract == "Laser")
            {
                Vector3 endPoint = Camera.main.transform.forward.normalized * distanceInteract + Camera.main.transform.position;
                if (Vector3.Distance(endPoint, pointInteract) != 0)
                {
                    directionInteract = endPoint - pointInteract;
                    _pointer.transform.rotation = Quaternion.LookRotation(directionInteract, math.RandomNormal(directionInteract));
                }
            }
            if (tagInteract == "Movable")
            {
                objectInteract.transform.position = Camera.main.transform.position + Camera.main.transform.forward.normalized * 2;
            }
        }
        if (Input.GetMouseButtonUp(1) && isInteraction)
        {
            isInteraction = false;
            Destroy(_pointer);
            pointInteract = objectInteract.transform.InverseTransformPoint(pointInteract);
            pointInteract = new Vector3(0, pointInteract.y, 0);
            pointInteract = objectInteract.transform.TransformPoint(pointInteract);

            Laser.PointDirection pd = new PointDirection(pointInteract, directionInteract);
            objectInteract.SendMessage("PointDirectionInteract", pd);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isInteraction == false)
            {
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 2f))
                {
                    if (hit.collider.tag == "Movable")
                    {
                        isInteraction = true;
                        objectInteract = hit.collider.gameObject;
                        pointInteract = hit.point;
                        distanceInteract = hit.distance;
                        tagInteract = hit.collider.tag;
                        rigidbodyInteract = objectInteract.GetComponent<Rigidbody>();
                        rigidbodyInteract.isKinematic = true;
                    }
                    else
                        isInteraction = false;
                }
                else
                    isInteraction = false;
            }
            else
            {
                rigidbodyInteract.isKinematic = false;
                isInteraction = false;
            }
        }
    }
}
