using UnityEngine;
using UnityEngine.InputSystem;

public sealed class NrpnReceiver : MonoBehaviour
{
    InputAction _actionParamMsb;
    InputAction _actionParamLsb;
    InputAction _actionParamData;

    (int msb, int lsb) _param;

    void OnParamMsbReceived(InputAction.CallbackContext ctx)
      => _param.msb = (int)(ctx.ReadValue<float>() * 128);

    void OnParamLsbReceived(InputAction.CallbackContext ctx)
      => _param.lsb = (int)(ctx.ReadValue<float>() * 128);

    void OnParamDataReceived(InputAction.CallbackContext ctx)
      => ProcessNrpnData(ctx.ReadValue<float>());

    static readonly string[] SynthParamNames
      = new[] { "Low Boost", "Pan", "Pitch", "Wave", "Mod Type", "Mod Speed",
                "Mod Depth", "Level", "Decay", "Motion Seq Type" };

    static readonly string[] PcmPartNames
      = new[] { "Close Hi-Hat", "Open Hi-Hat", "Crash", "H.Clap" };

    void ProcessSynthParam(int part, int kind, float data)
      => Debug.Log($"Synth{part + 1} {SynthParamNames[kind]} : {data}");

    void ProcessPcmParam(int part, int kind, float data)
      => Debug.Log($"{PcmPartNames[part]} {SynthParamNames[kind]} : {data}");

    void ProcessAudioInParam(int part, int kind, float data)
      => Debug.Log($"Audio{part} {SynthParamNames[kind]} : {data}");

    void ProcessMixerParam(int kind, float data)
    {
    }

    void ProcessNrpnData(float data)
    {
        if (_param.msb != 2) return;

        var lsb = _param.lsb;
        Debug.Log($"{lsb:X}");

        if (lsb < 4 * 10)
        {
            ProcessSynthParam(lsb / 10, lsb % 10, data);
            return;
        }

        lsb -= 4 * 10;

        if (lsb < 4 * 10)
        {
            ProcessPcmParam(lsb / 10, lsb % 10, data);
            return;
        }

        lsb -= 4 * 10;

        if (lsb < 2 * 10)
        {
            ProcessAudioInParam(lsb / 10, lsb % 10, data);
            return;
        }

        lsb -= 2 * 10;

        ProcessMixerParam(lsb, data);
    }

    void Start()
    {
        _actionParamMsb = new InputAction("NRPN MSB", binding: "<MidiDevice>/control099");
        _actionParamLsb = new InputAction("NRPN LSB", binding: "<MidiDevice>/control098");
        _actionParamData = new InputAction("NRPN Data", binding: "<MidiDevice>/control006");

        _actionParamMsb.performed += OnParamMsbReceived;
        _actionParamLsb.performed += OnParamLsbReceived;
        _actionParamData.performed += OnParamDataReceived;

        _actionParamMsb.canceled += OnParamMsbReceived;
        _actionParamLsb.canceled += OnParamLsbReceived;
        _actionParamData.canceled += OnParamDataReceived;

        _actionParamMsb.Enable();
        _actionParamLsb.Enable();
        _actionParamData.Enable();
    }
}
