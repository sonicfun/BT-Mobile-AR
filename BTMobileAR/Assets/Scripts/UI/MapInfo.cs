using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class MapInfo : MonoBehaviour {

    [SerializeField]
    private GameObject PrefabInfo;

    private CanvasShowInfo cinfo;

    private void Awake()
    {
        Assert.IsNotNull(PrefabInfo);
        var instance = Instantiate(PrefabInfo);
        cinfo = instance.GetComponent<CanvasShowInfo>();
    }

        // Use this for initialization
    void Start () {
   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (cinfo.IsVisible()) return;

                if (hit.transform.tag == "Pole"  || hit.transform.tag =="Node" || hit.transform.tag == "Cabinet")
                {
                    string sID = hit.transform.parent.name;
                    int ID = 0;
                    if (int.TryParse(sID, out ID) == false)
                        return;
                    List<BTCleanPOI>  lpoi =BTPOIManager.Instance.ListPoints;
                    BTCleanPOI pfound = lpoi.Where(x => x.ID == ID).FirstOrDefault();
                    if(pfound != null)
                        cinfo.DisplayCustomText(pfound.GetPoiDescriptionByScore());
                }
                else if(hit.transform.tag == "Player")
                {
                    cinfo.DisplayCustomText("This is Your Position!");
                }
            }
        }
    }
}
