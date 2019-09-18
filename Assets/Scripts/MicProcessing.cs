using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Linq;


public class MicProcessing : MonoBehaviour
{
    private FMOD.System system;

    FMOD.ChannelGroup channelMaster;
    FMOD.Channel channel;
    FMOD.DSP gate;
    FMOD.DSP pitchShift;
    FMOD.DSP lowpass;

    FMOD.REVERB_PROPERTIES reverbProperties;

    private int deviceIndex = 0; // Index 0 will typically be the input device currently active
    private int sampleRate = 0;
    private int channels = 0;

    private uint byteCount = 0;
    private byte[] bytes;
    private FMOD.Sound sound;
    private const float soundDuration = 5f; // The length of the recording in seconds

    private float onsetDelay = 0.2f;
    private bool isPlaying;

    private FMOD.VECTOR soundPosition;
    private FMOD.VECTOR soundVelocity;

    FMOD.DSP_METERING_INFO meteringInfo;
    private float[] inputData;
    private uint inputDataIndex = 0;
    public float inputLevel;

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
            Debug.Log("Drivers = " + numDrivers + ", Connected = " + numConnected);
            Debug.Log("Rate = " + sampleRate + ", Channels = " + channels);
        }

        byteCount = (uint)(soundDuration * 3.0f * (float)(channels * sampleRate)); // 3.0f for the byte size of 24 bit.
        bytes = new byte[byteCount];

        FMOD.CREATESOUNDEXINFO info = new FMOD.CREATESOUNDEXINFO();
        info.cbsize = Marshal.SizeOf(info);
        info.numchannels = channels;
        info.defaultfrequency = sampleRate;
        info.format = FMOD.SOUND_FORMAT.PCM24;
        info.length = byteCount;

        reverbProperties = FMOD.PRESET.CONCERTHALL();
        system.setReverbProperties(0, ref reverbProperties);

        FMODCheck(system.createSound(bytes, FMOD.MODE.LOOP_NORMAL | FMOD.MODE.OPENUSER | FMOD.MODE._3D, ref info, out sound));

        FMODCheck(system.recordStart(deviceIndex, sound, true));
    }

    void Update()
    {
        if (!isPlaying & Time.time > onsetDelay)
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
        pitchShift.setMeteringEnabled(true, true);

        FMODCheck(system.createDSPByType(FMOD.DSP_TYPE.LOWPASS_SIMPLE, out lowpass));
        lowpass.setParameterFloat(0, 2000f); // Set lowpass cutoff

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