using System;
using Unity.Mathematics;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PerceptionSensor : ISensor
{
    private SightDetector m_Detector;
    private string m_name;

    public PerceptionSensor(SightDetector detector, string name)
    {
        m_Detector = detector;
        m_name = name;
    }
    public int Write(ObservationWriter writer)
    {
        bool seePlayer = m_Detector.IsTargetVisible;
        Vector3 relativePosition = m_Detector.targetPosition - m_Detector.origin.position;
        writer[0] = seePlayer ? 1.0f : 0.0f;
        if (seePlayer)
        {
            float normalizedX = Mathf.Clamp(relativePosition.x / 30, -1.0f, 1.0f);
            float normalizedY = Mathf.Clamp(relativePosition.y / 5, -1.0f, 1.0f);
            float normalizedZ = Mathf.Clamp(relativePosition.z / 30, -1.0f, 1.0f);
            writer[1] = normalizedX;
            writer[2] = normalizedY;
            writer[3] = normalizedZ;
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"Sensor Output: X={normalizedX} | Y={normalizedY} | Z={normalizedZ} | InRange={seePlayer}");
            }
            return 4;
        }
        return 1;
    }

    public byte[] GetCompressedObservation()
    { return null; }
    public CompressionSpec GetCompressionSpec()
    {
        return CompressionSpec.Default();
    }
    public string GetName()
    {
        return m_name;
    }
    public ObservationSpec GetObservationSpec()
    {
        return ObservationSpec.Vector(4);
    }
    public void Reset()
    {
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ISensor.Update()
    {
        Update();
    }
}
