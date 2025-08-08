using UnityEngine;
using UnityEngine.Rendering;

public class Slingshot : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;


    [Header("Dynamic")]
    // Dynamic fields
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    private LensFlareComponentSRP lfSRP;
    private void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        //launchPoint.SetActive(false);
        lfSRP =launchPoint.GetComponent<LensFlareComponentSRP>();
        lfSRP.enabled = false;
        launchPos = launchPointTrans.position;
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

    /// <summary>
    /// Create a new projectile at the launch position
    /// and turn off it's response to normal physics.
    /// </summary>
    void OnMouseDown()
    {
        aimingMode = true;
        projectile = Instantiate(projectilePrefab) as GameObject;
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;

    }


    /// <summary>
    /// Let the user pull the projectile and eventually release it.
    /// The script must be in aimingMode (true)
    /// </summary>
    void Update()
    {
        if (!aimingMode) return;

        // Get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;
        // Adjust the z position to be opposite the camera's z.
        mousePos2D.z = -Camera.main.transform.position.z;

        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        // Get the difference vector between the mouse and the launch position.
        Vector3 mouseDelta = mousePos3D - launchPos;
        // Limit mouseDelta to the radius of the Slingshot Sphere
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            // Scale within 1 and then scale by the radius.
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        // Move the projectile to this new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        // NOTE: AT this point test that the projectile is moved to the mouse position 
        // and is constrained.

        // Release the projectile with velocity moving the projectile in the opposite direction.
        // Setting the field projectile back to null severs the connection between this instance of the Slingshot script and the projectile GameObject.
        if (Input.GetMouseButtonUp(0))
        {
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.linearVelocity = -mouseDelta * velocityMult;
            projectile = null;
        }

    }
}
