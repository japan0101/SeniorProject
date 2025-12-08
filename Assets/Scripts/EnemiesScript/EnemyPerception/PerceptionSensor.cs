using System;
using Unity.Mathematics;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
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
        try
        {
            if (m_Detector == null || m_Detector.isShuttingDown || m_Detector.origin == null)
            {
                writer[0] = 0f;
                writer[1] = 0f;
                writer[2] = 0f;
                writer[3] = 0f;
                return 4;
            }

            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"writing observations");
            }
            bool seePlayer = m_Detector.IsTargetVisible;
            writer[0] = seePlayer ? 1.0f : 0.0f;

            if (seePlayer)
            {

                Vector3 relativePosition = m_Detector.targetPosition - m_Detector.origin.position;

                //Normalized relative position
                float normalizedX = relativePosition.x / m_ObservationRange.x;
                float normalizedY = relativePosition.y / m_ObservationRange.y;
                float normalizedZ = relativePosition.z / m_ObservationRange.z;

                //Clean bad data ie.
                if (float.IsNaN(normalizedX) || float.IsInfinity(normalizedX)) normalizedX = 0f;
                if (float.IsNaN(normalizedY) || float.IsInfinity(normalizedY)) normalizedY = 0f;
                if (float.IsNaN(normalizedZ) || float.IsInfinity(normalizedZ)) normalizedZ = 0f;

                writer[1] = Mathf.Clamp(normalizedX, -1.0f, 1.0f);
                writer[2] = Mathf.Clamp(normalizedY, -1.0f, 1.0f);
                writer[3] = Mathf.Clamp(normalizedZ, -1.0f, 1.0f);
                if (Time.frameCount % 30 == 0)
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
        }
        catch (Exception)
        {
            // If ANYTHING goes wrong (Shutdown, NullRef, Native Access), 
            // just write zeros to keep the engine alive.
            WriteZeros(writer);
        }

        //In case there are no detectors
        return 4;
    }
    private void WriteZeros(ObservationWriter writer)
    {
        writer[0] = 0f;
        writer[1] = 0f;
        writer[2] = 0f;
        writer[3] = 0f;
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
