using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;

public class SpawnSolarPanel: MonoBehaviour
{
    [SerializeField] private GameObject _solarPanelToSpawn; // assign the prefab to spawn in the inspector
    [SerializeField] private float _spawnDistance = 200f; // distance to spawn in front of the camera

    private Camera mainCamera;

    private void Start()
    {
        _solarPanelToSpawn.GetComponent<SolarPanel>();

        CameraOffset cameraOffset = GetComponentInChildren<CameraOffset>();
        if (cameraOffset != null)
        {
            mainCamera = cameraOffset.GetComponentInChildren<Camera>();
        }
        else
        {
            mainCamera = GetComponentInChildren<Camera>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * _spawnDistance; // spawn in front of the camera
            Quaternion spawnRotation = Quaternion.identity; // use default rotation


            GameObject newObject = Instantiate(_solarPanelToSpawn, spawnPosition, spawnRotation);
        }
    }
}
