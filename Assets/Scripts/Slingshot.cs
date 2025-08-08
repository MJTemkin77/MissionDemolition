using UnityEngine;
using UnityEngine.Rendering;

public class Slingshot : MonoBehaviour
{
    private GameObject launchPoint;
    private LensFlareComponentSRP lfSRP;
    private void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        //launchPoint.SetActive(false);
        lfSRP =launchPoint.GetComponent<LensFlareComponentSRP>();
        lfSRP.enabled = false;
    }
    void OnMouseEnter()
    {
        print("Slingshot:OnMouseEnter()");
        //launchPoint.SetActive(true);
        lfSRP.enabled =  true;
    }
    void OnMouseExit()
    {
        print("Slingshot:OnMouseExit()");
        //launchPoint.SetActive(false);
        lfSRP.enabled = false;
    }
}
