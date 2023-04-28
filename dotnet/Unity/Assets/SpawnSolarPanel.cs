using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;

public class SpawnSolarPanel: MonoBehaviour
{
    [SerializeField] private GameObject _solarPanelToSpawn; 
    [SerializeField] private float _spawnDistance = 200f; // distance to spawn in front of the camera

    private Camera mainCamera;

    private void Start()
    {
        var roofCollider = GameObject.Find("RoofCollider").GetComponent<BoxCollider>();
        _solarPanelToSpawn.GetComponent<SolarPanel>().SetSnapPlane(roofCollider);


        CameraOffset cameraOffset = GetComponentInChildren<CameraOffset>();
        if (cameraOffset != null) 
            mainCamera = cameraOffset.GetComponentInChildren<Camera>();
        else 
            mainCamera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * _spawnDistance; // spawn in front of the camera
            Quaternion spawnRotation = Quaternion.identity; // use default rotation

            Instantiate(_solarPanelToSpawn, spawnPosition, spawnRotation);
        }
    }
}
