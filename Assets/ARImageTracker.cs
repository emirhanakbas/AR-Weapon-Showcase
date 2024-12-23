using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class ARImageTracker : MonoBehaviour
{
    [System.Serializable]
    public class WeaponData
    {
        public string qrCodeName;      // QR code name (Reference Image Name).
        public GameObject weaponPrefab; // Weapon prefab.
    }

    public ARTrackedImageManager trackedImageManager; // ARTrackedImageManager component.
    public WeaponData[] weapons;                     // QR code - prefab match-up.
    public Button rotateLeftButton;                  // Rotate Left button.
    public Button rotateRightButton;                 // Rotate Right button.
    public Button rotateUpButton;                    // Rotate Up button.
    public Button rotateDownButton;                  // Rotate Down button.
    public Button zoomInButton;                      // Zoom In button.
    public Button zoomOutButton;                     // Zoom Out button.
    public float rotationSpeed = 10f;                // Rotation speed.
    public float zoomSpeed = 0.1f;                   // Zoom speed.

    private Dictionary<string, GameObject> spawnedWeapons = new Dictionary<string, GameObject>();
    private GameObject currentWeapon;

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        rotateLeftButton.onClick.AddListener(RotateLeft);
        rotateRightButton.onClick.AddListener(RotateRight);
        rotateUpButton.onClick.AddListener(RotateUp); 
        rotateDownButton.onClick.AddListener(RotateDown); 
        zoomInButton.onClick.AddListener(ZoomIn);
        zoomOutButton.onClick.AddListener(ZoomOut);
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        rotateLeftButton.onClick.RemoveListener(RotateLeft);
        rotateRightButton.onClick.RemoveListener(RotateRight);
        rotateUpButton.onClick.RemoveListener(RotateUp); 
        rotateDownButton.onClick.RemoveListener(RotateDown); 
        zoomInButton.onClick.RemoveListener(ZoomIn);
        zoomOutButton.onClick.RemoveListener(ZoomOut);
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
        {
            RemoveAllWeapons();
            SpawnWeapon(trackedImage);
        }

        foreach (var trackedImage in args.updated)
        {
            if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                UpdateWeaponPosition(trackedImage);
            }
        }

        foreach (var trackedImage in args.removed)
        {
            RemoveWeapon(trackedImage);
        }
    }

    private void SpawnWeapon(ARTrackedImage trackedImage)
    {
        string qrCodeName = trackedImage.referenceImage.name;

        foreach (var weapon in weapons)
        {
            if (weapon.qrCodeName == qrCodeName)
            {
                if (!spawnedWeapons.ContainsKey(qrCodeName))
                {
                    currentWeapon = Instantiate(weapon.weaponPrefab, trackedImage.transform.position, Quaternion.identity);
                    currentWeapon.transform.rotation = Quaternion.LookRotation(-trackedImage.transform.up);
                    currentWeapon.transform.localScale = Vector3.one * 0.5f;
                    currentWeapon.transform.SetParent(trackedImage.transform);

                    spawnedWeapons.Add(qrCodeName, currentWeapon);
                }
            }
        }
    }

    private void UpdateWeaponPosition(ARTrackedImage trackedImage)
    {
        string qrCodeName = trackedImage.referenceImage.name;
        if (spawnedWeapons.ContainsKey(qrCodeName))
        {
            spawnedWeapons[qrCodeName].transform.position = trackedImage.transform.position;
        }
    }

    private void RemoveWeapon(ARTrackedImage trackedImage)
    {
        string qrCodeName = trackedImage.referenceImage.name;
        if (spawnedWeapons.ContainsKey(qrCodeName))
        {
            Destroy(spawnedWeapons[qrCodeName]);
            spawnedWeapons.Remove(qrCodeName);
        }
    }

    private void RemoveAllWeapons()
    {
        foreach (var weapon in spawnedWeapons.Values)
        {
            Destroy(weapon);
        }
        spawnedWeapons.Clear();
    }

    public void RotateLeft()
    {
        if (currentWeapon != null)
        {
            currentWeapon.transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
        }
    }

    public void RotateRight()
    {
        if (currentWeapon != null)
        {
            currentWeapon.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }

    public void RotateUp()
    {
        if (currentWeapon != null)
        {
            currentWeapon.transform.Rotate(Vector3.right * -rotationSpeed * Time.deltaTime);
        }
    }

    public void RotateDown()
    {
        if (currentWeapon != null)
        {
            currentWeapon.transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        }
    }

    public void ZoomIn()
    {
        if (currentWeapon != null)
        {
            currentWeapon.transform.localScale += Vector3.one * zoomSpeed;
        }
    }

    public void ZoomOut()
    {
        if (currentWeapon != null)
        {
            currentWeapon.transform.localScale -= Vector3.one * zoomSpeed;
        }
    }
}
