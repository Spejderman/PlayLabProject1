using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Linq;

/// <summary>
/// Records audio from the default device microphone, adds DSP effects and plays it back – continuously. 
/// REMEMBER: When using FMOD, Unity Audio should be disabled in project setting.
/// </summary>
public class MicProcessing : MonoBehaviour
{
    private FMOD.System system;

    // Audio configuration
    private int deviceIndex = 0; // Index 0 will typically be the input device currently active
    private int sampleRate = 0;
    private int channels = 0;

    public float userLatency = 200f; // User latency in ms
    private bool isPlaying;

    // FMOD sound configuration
    private uint byteCount = 0;
    private byte[] bytes;
    private FMOD.Sound sound;
    private const float soundDuration = 5f; // The length of the recording in seconds

    private FMOD.VECTOR soundPosition;
    private FMOD.VECTOR soundVelocity;

    // DSP
    FMOD.ChannelGroup channelMaster;
    FMOD.Channel channel;
    FMOD.DSP gate;
    FMOD.DSP pitchShift;
    FMOD.DSP lowpass;

    FMOD.REVERB_PROPERTIES reverbProperties;

    // Metering
    FMOD.DSP_METERING_INFO meteringInfo;
    private float[] inputData;
    private uint inputDataIndex = 0;
    public float inputLevel;

    // Debug
    public bool debug;

    void Start()
    {
        system = FMODUnity.RuntimeManager.CoreSystem;

        inputData = new float[100];

        int numDrivers, numConnected;
        FMODCheck(system.getRecordNumDrivers(out numDrivers, out numConnected));

        Guid guid;
        string name;
        FMOD.SPEAKERMODE speakerMode;
        FMOD.DRIVER_STATE driverState;
        FMODCheck(system.getRecordDriverInfo(deviceIndex, out name, 0, out guid, out sampleRate, out speakerMode, out channels, out driverState));

        if (debug)
        {
            Debug.Log($"There are {numDrivers} drivers, and {numConnected} are connected:");

            foreach (var device in Microphone.devices)
                Debug.Log(device);

            Debug.Log($"The selected driver is {Microphone.devices[deviceIndex]}");
            Debug.Log($"The selected driver has a sample rate of {sampleRate} Hz");
            Debug.Log($"The selected driver has {channels} channels");
        }

        byteCount = (uint)(soundDuration * 2.0f * (float)(channels * sampleRate)); // 2.0f for the byte size of 16 bit
        bytes = new byte[byteCount];

        FMOD.CREATESOUNDEXINFO info = new FMOD.CREATESOUNDEXINFO();
        info.cbsize = Marshal.SizeOf(info);
        info.numchannels = channels;
        info.defaultfrequency = sampleRate;
        info.format = FMOD.SOUND_FORMAT.PCM16;
        info.length = byteCount;

        reverbProperties = FMOD.PRESET.CONCERTHALL();
        system.setReverbProperties(0, ref reverbProperties);

        FMODCheck(system.createSound(bytes, FMOD.MODE.LOOP_NORMAL | FMOD.MODE.OPENUSER | FMOD.MODE._3D, ref info, out sound));

        // Start recording
        if (debug)
            Debug.Log("Starting recording");

        FMODCheck(system.recordStart(deviceIndex, sound, true));
    }

    void Update()
    {
        if (!isPlaying & Time.time > userLatency / 1000)
            FMODInit();

        if (isPlaying)
        {
            FMODCheck(pitchShift.getMeteringInfo(IntPtr.Zero, out meteringInfo));
            inputLevel = SmoothRms(meteringInfo.rmslevel[0]);
        }
    }

    /// <summary>
    /// Calculates a running average of RMS values. Window size = 100.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private float SmoothRms(float value)
    {
        inputData[inputDataIndex % 100] = value;
        inputDataIndex++;

        return inputData.Average();
    }

    /// <summary>
    /// Starts playback of recorded audio with DSP effects added. 
    /// </summary>
    private void FMODInit()
    {
        system.getMasterChannelGroup(out channelMaster);

        FMODCheck(system.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT, out pitchShift));
        pitchShift.setParameterFloat(0, 0.5f); // Set pitch shift
        pitchShift.setParameterFloat(1, 512f); // Set FFT window size
        pitchShift.setMeteringEnabled(true, true);

        FMODCheck(system.createDSPByType(FMOD.DSP_TYPE.LOWPASS_SIMPLE, out lowpass));
        lowpass.setParameterFloat(0, 2000f); // Set lowpass cutoff

        // Start playback
        if (debug)
            Debug.Log("Starting playback");

        FMODCheck(system.playSound(sound, channelMaster, false, out channel));
        channel.set3DAttributes(ref soundPosition, ref soundVelocity);

        FMODCheck(channel.addDSP(1, lowpass));
        FMODCheck(channel.addDSP(2, pitchShift));

        channel.isPlaying(out isPlaying);
    }

    /// <summary>
    /// Checks if the result of a FMOD operation is OK.
    /// If not, outputs the error message.
    /// </summary>
    /// <param name="result"></param>
    private void FMODCheck(FMOD.RESULT result)
    {
        if (result != FMOD.RESULT.OK)
            Debug.Log("FMOD Error = " + result);
    }
}