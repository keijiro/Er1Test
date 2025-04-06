using UnityEngine;
using UnityEngine.InputSystem;

public sealed class NrpnReceiver : MonoBehaviour
{
    #region Input action callbacks

    InputAction _actionParamMsb;
    InputAction _actionParamLsb;
    InputAction _actionParamData;

    void OnParamMsbReceived(InputAction.CallbackContext ctx)
      => _param.msb = (int)(ctx.ReadValue<float>() * 128);

    void OnParamLsbReceived(InputAction.CallbackContext ctx)
      => _param.lsb = (int)(ctx.ReadValue<float>() * 128);

    void OnParamDataReceived(InputAction.CallbackContext ctx)
      => ProcessNrpnData(ctx.ReadValue<float>());

    #endregion

    #region MonoBehaviour implementation

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

    #endregion

    #region String table

    static readonly string[] PartNames
      = new[] { "Synth1", "Synth2", "Synth3", "Synth4",
                "Close Hi-Hat", "Open Hi-Hat", "Crash", "H.Clap",
                "Audio1", "Audio2" };

    static readonly string[] PartParamNames
      = new[] { "Low Boost", "Pan", "Pitch", "Wave", "Mod Type", "Mod Speed",
                "Mod Depth", "Level", "Decay", "Motion Seq Type" };

    static readonly string[] GlobalParamNames
      = new[] { "Delay Depth", "Delay Time", "Cross (Synth1 & Synth2)",
                "Ring (Synth4 & Audio In)", "Input Gain 1", "Input Gain 2",
                "Accent Level", "Delay Type", "", "Mute 1", "Mute 2" };

    #endregion

    #region NRPN data processing

    (int msb, int lsb) _param;

    void ProcessNrpnData(float data)
    {
        if (_param.msb != 2) return;

        var lsb = _param.lsb;

        if (lsb < 100)
        {
            var partName = PartNames[lsb / 10];
            var paramName = PartParamNames[lsb % 10];
            Debug.Log($"{partName} {paramName} : {data}");
        }
        else
        {
            var paramName = GlobalParamNames[lsb - 100];
            Debug.Log($"{paramName} : {data}");
        }
    }

    #endregion
}
