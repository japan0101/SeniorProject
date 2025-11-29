using Unity.MLAgents.Sensors;
using UnityEngine;

public class PerceptionSensorComponent : SensorComponent
{
    public string sensorName = "SightSensor";
    public SightDetector sightDetector;
    public override ISensor[] CreateSensors()
    {
        return new ISensor[] { new PerceptionSensor(sightDetector, sensorName) };
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
