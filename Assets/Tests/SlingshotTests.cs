using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SlingshotTests
{
    private /*IEnumerator*/ void CreateSlingshot(out Slingshot slingshot, out GameObject projectilePrefab, out GameObject camGO, out GameObject launchPoint)
    {
        // Create and tag a main camera required by Slingshot.Update()
        camGO = new GameObject("MainCamera");
        camGO.tag = "MainCamera";
        camGO.AddComponent<Camera>();

        // Create the Slingshot host GameObject and required collider
        GameObject slingGO = new GameObject("SlingshotGO");
        var sphere = slingGO.AddComponent<SphereCollider>();
        sphere.radius = 1f;

        // Create the LaunchPoint child before adding Slingshot so Awake() can find it
        launchPoint = new GameObject("LaunchPoint");
        launchPoint.transform.SetParent(slingGO.transform);
        launchPoint.transform.localPosition = new Vector3(1.23f, 4.56f, 7.89f);
        launchPoint.AddComponent(typeof(UnityEngine.Rendering.LensFlareComponentSRP));

        // Add Slingshot (Awake runs immediately and will configure lfSRP + launchPos)
        slingshot = slingGO.AddComponent<Slingshot>();

        // Provide a projectile prefab with a Rigidbody as required by OnMouseDown()
        projectilePrefab = new GameObject("ProjectilePrefab");
        projectilePrefab.AddComponent<Rigidbody>();
        slingshot.projectilePrefab = projectilePrefab;

        // Let Unity process initializations
        //yield return null;
    }

    private void Cleanup(params Object[] objects)
    {
        foreach (var obj in objects)
        {
            if (obj != null)
            {
                Object.DestroyImmediate(obj);
            }
        }
    }

    [UnityTest]
    public IEnumerator Awake_Initializes_LaunchPoint_And_LensFlare_Disabled()
    {
        
        CreateSlingshot(out var slingshot, out var projectilePrefab, out var camGO, out var launchPoint);

        var lf = launchPoint.GetComponent<UnityEngine.Rendering.LensFlareComponentSRP>();

        Assert.IsNotNull(slingshot.launchPoint, "launchPoint should be assigned in Awake()");
        Assert.AreEqual(launchPoint, slingshot.launchPoint, "launchPoint should reference the LaunchPoint child");
        Assert.AreEqual(launchPoint.transform.position, slingshot.launchPos, "launchPos should match LaunchPoint position");
        Assert.IsFalse(lf.enabled, "Lens flare should be disabled initially");

        Cleanup(slingshot.gameObject, projectilePrefab, camGO);
        yield return null;
    }

    [UnityTest]
    public IEnumerator OnMouseEnter_Enables_LensFlare()
    {
        CreateSlingshot(out var slingshot, out var projectilePrefab, out var camGO, out var launchPoint);
        var lf = launchPoint.GetComponent<UnityEngine.Rendering.LensFlareComponentSRP>();

        slingshot.gameObject.SendMessage("OnMouseEnter");
        yield return null;

        Assert.IsTrue(lf.enabled, "Lens flare should be enabled on mouse enter");

        Cleanup(slingshot.gameObject, projectilePrefab, camGO);
    }

    [UnityTest]
    public IEnumerator OnMouseExit_Disables_LensFlare()
    {
        CreateSlingshot(out var slingshot, out var projectilePrefab, out var camGO, out var launchPoint);
        var lf = launchPoint.GetComponent<UnityEngine.Rendering.LensFlareComponentSRP>();

        slingshot.gameObject.SendMessage("OnMouseEnter");
        yield return null;
        Assert.IsTrue(lf.enabled, "Lens flare should be enabled before exit");

        slingshot.gameObject.SendMessage("OnMouseExit");
        yield return null;

        Assert.IsFalse(lf.enabled, "Lens flare should be disabled on mouse exit");

        Cleanup(slingshot.gameObject, projectilePrefab, camGO);
    }

    [UnityTest]
    public IEnumerator OnMouseDown_Instantiates_Projectile_At_LaunchPos_Kinematic_And_AimingMode()
    {
        CreateSlingshot(out var slingshot, out var projectilePrefab, out var camGO, out var launchPoint);

        slingshot.gameObject.SendMessage("OnMouseDown");
        yield return null;

        Assert.IsNotNull(slingshot.projectile, "Projectile should be instantiated on mouse down");
        Assert.AreEqual(slingshot.launchPos, slingshot.projectile.transform.position, "Projectile should spawn at launchPos");
        var rb = slingshot.projectile.GetComponent<Rigidbody>();
        Assert.IsNotNull(rb, "Projectile should have a Rigidbody");
        Assert.IsTrue(rb.isKinematic, "Projectile Rigidbody should be kinematic while aiming");
        Assert.IsTrue(slingshot.aimingMode, "aimingMode should be true after mouse down");

        Cleanup(slingshot.gameObject, projectilePrefab, camGO, slingshot.projectile);
    }

    [UnityTest]
    public IEnumerator OnMouseDown_Multiple_Creates_New_Projectiles()
    {
        CreateSlingshot(out var slingshot, out var projectilePrefab, out var camGO, out var launchPoint);

        slingshot.gameObject.SendMessage("OnMouseDown");
        yield return null;
        var first = slingshot.projectile;

        slingshot.gameObject.SendMessage("OnMouseDown");
        yield return null;
        var second = slingshot.projectile;

        Assert.AreNotSame(first, second, "Subsequent mouse downs should create a new projectile instance");

        Cleanup(slingshot.gameObject, projectilePrefab, camGO, first, second);
    }
}