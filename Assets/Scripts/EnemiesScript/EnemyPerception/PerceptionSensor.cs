using System;
using Unity.Mathematics;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PerceptionSensor : ISensor
{
    private SightDetector m_Detector;
    private string m_name;
    private Vector3 m_ObservationRange;

    public PerceptionSensor(SightDetector detector, string name)
    {
        m_Detector = detector;
        m_name = name;
        m_ObservationRange = new Vector3(30f, 5f, 30f);
    }
    public PerceptionSensor(SightDetector detector, string name, Vector3 ObservationRange)
    {
        m_Detector = detector;
        m_name = name;
        m_ObservationRange = ObservationRange;
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
        }
        else
        {
            writer[1] = 0.0f;
            writer[2] = 0.0f;
            writer[3] = 0.0f;
        }
        return 4;
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
    }

    void ISensor.Update()
    {
    }
}
