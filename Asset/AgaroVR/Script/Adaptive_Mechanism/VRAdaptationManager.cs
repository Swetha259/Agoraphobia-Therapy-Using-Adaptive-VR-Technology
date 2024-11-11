using UnityEngine;
using System.IO.Ports;

public class VRAdaptationManager : MonoBehaviour {
    private SerialPort serialPort = new SerialPort("COM3", 9600); // Set the correct COM port and baud rate
    private float heartRate;
    
    // Constants for the intensity formula
    private const float BaseIntensity = 1.0f;
    private const float BaselineHeartRate = 70.0f; // Customize based on user baseline
    private const float Sensitivity = 0.02f; // Adjust sensitivity as needed

    public AudioSource crowdNoiseSource; // Attach your audio source for crowd noise
    public Light sceneLight; // Attach a light in the scene to simulate lighting effects

    void Start() {
        serialPort.Open();
    }

    void Update() {
        // Check if there's data from the Arduino
        if (serialPort.IsOpen && serialPort.BytesToRead > 0) {
            try {
                string data = serialPort.ReadLine(); // Read data from Arduino
                heartRate = float.Parse(data); // Parse heart rate
                AdjustVREnvironment(heartRate); // Adjust the VR environment
            } catch (System.Exception e) {
                Debug.LogWarning("Error reading data: " + e.Message);
            }
        }
    }

    void AdjustVREnvironment(float hr) {
        // Calculate Intensity Factor (IF) based on heart rate
        float intensityFactor = BaseIntensity - Sensitivity * (hr - BaselineHeartRate);

        // Clamp intensityFactor between 0 and 1 to avoid negative or excessive values
        intensityFactor = Mathf.Clamp(intensityFactor, 0.0f, 1.0f);

        // Adjust crowd noise volume based on intensity factor
        if (crowdNoiseSource != null) {
            crowdNoiseSource.volume = intensityFactor; // Scale volume with intensity factor
        }

        // Adjust lighting brightness based on intensity factor
        if (sceneLight != null) {
            sceneLight.intensity = intensityFactor * 2.0f; // Adjust multiplier for brightness
        }

        // (Optional) Debugging output in Unity console
        Debug.Log($"Heart Rate: {hr}, Intensity Factor: {intensityFactor}");
    }

    void OnApplicationQuit() {
        // Close the serial port when the application quits
        if (serialPort.IsOpen) {
            serialPort.Close();
        }
    }
}
