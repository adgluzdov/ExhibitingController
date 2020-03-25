using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExhibitingController : MonoBehaviour
{
    public Camera camera;
    public GameObject prefab;
    public EventTrigger trigger;
    public float height;

    private DragStatus dragStatus;
    private PointerStatus pointerStatus;
    private GameObject instance;
    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        dragStatus = DragStatus.None;
        pointerStatus = PointerStatus.None;

        EventTrigger.Entry entry;

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.BeginDrag;
        entry.callback.AddListener((data) =>
        {
            dragStatus = DragStatus.Begin;
        });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.EndDrag;
        entry.callback.AddListener((data) =>
        {
            dragStatus = DragStatus.End;
        });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) =>
        {
            pointerStatus = PointerStatus.Enter;
        });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) =>
        {
            pointerStatus = PointerStatus.Exit;
        });
        trigger.triggers.Add(entry);
    }

    // Update is called once per frame
    void Update()
    {
        if (pointerStatus == PointerStatus.Exit && dragStatus == DragStatus.Begin)
        {
            int layerMask = ~(1 << 10);
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Vector3 position = ray.GetPoint(hit.distance);
                Quaternion rotation = Quaternion.LookRotation(camera.transform.position - position, Vector3.up);
                if (instance == null)
                {
                    instance = GameObject.Instantiate<GameObject>(prefab.gameObject);
                    rigidbody = instance.GetComponent<Rigidbody>();
                    rigidbody.useGravity = false;
                    rigidbody.isKinematic = true;
                }
                instance.transform.position = position + Vector3.up * height;
                instance.transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
                instance.SetActive(true);
            }
        }
        if (pointerStatus == PointerStatus.Exit && dragStatus == DragStatus.End && instance != null)
        {
            if (instance.activeSelf == false)
            {
                Destroy(instance);
            }
            else
            {
                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
                instance = null;
                rigidbody = null;
            }
        }

        if (pointerStatus == PointerStatus.Enter && dragStatus == DragStatus.End && instance != null)
        {
            Destroy(instance);
        }

        if (pointerStatus == PointerStatus.Enter && dragStatus == DragStatus.Begin && instance != null)
        {
            instance.SetActive(false);
        }
    }

    public enum DragStatus
    {
        None = 0,
        Begin = 1,
        End = 2
    }

    public enum PointerStatus
    {
        None = 0,
        Enter = 1,
        Exit = 2
    }


}


