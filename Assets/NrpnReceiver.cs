using UnityEngine;
using UnityEngine.InputSystem;

public sealed class NrpnReceiver : MonoBehaviour
{
    InputAction _actionParamMsb;
    InputAction _actionParamLsb;
    InputAction _actionParamData;

    (int msb, int lsb) _param;

    void OnParamMsbReceived(InputAction.CallbackContext ctx)
      => _param.msb = (int)(ctx.ReadValue<float>() * 127);

    void OnParamLsbReceived(InputAction.CallbackContext ctx)
      => _param.lsb = (int)(ctx.ReadValue<float>() * 127);

    void OnParamDataReceived(InputAction.CallbackContext ctx)
      => Debug.Log($"{_param.msb} {_param.lsb} {ctx.ReadValue<float>()}");

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
