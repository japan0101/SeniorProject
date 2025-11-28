using Unity.MLAgents.Sensors;
using UnityEngine;

public class PerceptionSensor : ISensor
{
    public byte[] GetCompressedObservation()
    {
        throw new System.NotImplementedException();
    }

    public CompressionSpec GetCompressionSpec()
    {
        throw new System.NotImplementedException();
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }

    public ObservationSpec GetObservationSpec()
    {
        throw new System.NotImplementedException();
    }

    public void Reset()
    {
        throw new System.NotImplementedException();
    }

    public int Write(ObservationWriter writer)
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
