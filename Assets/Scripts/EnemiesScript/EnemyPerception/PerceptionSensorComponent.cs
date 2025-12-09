using Unity.MLAgents.Sensors;
using UnityEngine;

public class PerceptionSensorComponent : SensorComponent
{
    [Header("Sensor Settings")]
    public string sensorName = "SightSensor";

    [Header("Dependencies")]
    public SightDetector sightDetector;

    [Header("Normalization Settings")]
    [Tooltip("Matches the logic values: X=30, Y=5, Z=30")]
    public Vector3 observationRange = new Vector3(30f, 5f, 30f);
    public override ISensor[] CreateSensors()
    {
        if (sightDetector == null)
        {
            sightDetector = GetComponentInChildren<SightDetector>();
            if (sightDetector == null)
            {
                Debug.LogError($"[CRITICAL] SightDetector missing on {name}. Sensor will be disabled to prevent crash.");
                return new ISensor[0];
            }
        }

        // Validate the range to prevent division by zero
        if (observationRange == Vector3.zero) observationRange = Vector3.one;

        return new ISensor[] { new PerceptionSensor(sightDetector, sensorName, observationRange) };
    }
}
