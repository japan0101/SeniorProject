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
            Debug.LogError("SightDetector is missing on " + name + ". Please assign it in the Inspector!");
            return new ISensor[0];
        }
        return new ISensor[] { new PerceptionSensor(sightDetector, sensorName, observationRange) };
    }
}
