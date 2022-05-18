using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;

public class MyKeyboard : MonoBehaviour
{
    [SerializeField]
    private float Hl1Distance = 1.0f;
    [SerializeField]
    private float Hl1Scale = 1.0f;

    [SerializeField]
    private float Hl2Distance = 0.3f;
    [SerializeField]
    private float Hl2Scale = 0.3f;

    [SerializeField]
    private float WmrHeadSetDistance = 0.6f;

    [SerializeField]
    private float WmrHeadSetScale = 0.6f;

    [SerializeField]
    private AudioClip _clickSound;

    private AudioSource _clickSoundPlayer;

    //add
    public TMP_InputField InputField = null;

    public NonNativeKeyboard nonNativeKeyboard;
    //add

    private void Start()
    {
        _clickSoundPlayer = gameObject.AddComponent<AudioSource>();
        _clickSoundPlayer.playOnAwake = false;
        _clickSoundPlayer.spatialize = true;
        _clickSoundPlayer.clip = _clickSound;
        var buttons = GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            var ni = button.gameObject.AddComponent<NearInteractionTouchableUnityUI>();
            ni.EventsToReceive = TouchableEventType.Pointer;
            button.onClick.AddListener(PlayClick);
        }
        //add
        nonNativeKeyboard.OnTextSubmitted += OnTextSubmitted;
        //add
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log(GetInputFieldText());
        }
    }

    private void PlayClick()
    {
        if (_clickSound != null)
        {
            _clickSoundPlayer.Play();
        }
    }

    private float Scale => GetPlatformValue(Hl1Scale, Hl2Scale, WmrHeadSetScale);
    private float Distance => GetPlatformValue(Hl1Distance, Hl2Distance, WmrHeadSetDistance);

    private float GetPlatformValue(float hl1Value, float hl2Value, float wmrHeadsetValue)
    {
        if (CoreServices.CameraSystem.IsOpaque)
        {
            return wmrHeadsetValue;
        }

        var capabilityChecker = CoreServices.InputSystem as IMixedRealityCapabilityCheck;

        return capabilityChecker.CheckCapability(MixedRealityCapability.ArticulatedHand) ?
                hl2Value : hl1Value;
    }

    public void ShowKeyboard()
    {
        NonNativeKeyboard.Instance.PresentKeyboard();
        NonNativeKeyboard.Instance.RepositionKeyboard(CameraCache.Main.transform.position +
                                                      CameraCache.Main.transform.forward *
                                                      Distance, 0f);
        NonNativeKeyboard.Instance.gameObject.transform.localScale *= Scale;
    }


    public string GetInputFieldText()
    {
        return InputField.text.ToString();
    }

    /// <summary>
    /// from NonNativeKeyboard class:
    ///   public event EventHandler OnTextSubmitted = delegate { };
    /// Sent when the 'Enter' button is pressed. To retrieve the text from the event,
    /// cast the sender to 'Keyboard' and get the text from the TextInput field.
    /// </summary>
    void OnTextSubmitted(object sender, EventArgs args)
    {
        Debug.Log("Enter&Sent!!!!!!!!!!!!!!!!!!!!!!!!");

        NonNativeKeyboard keyboard = (NonNativeKeyboard)sender;
        Debug.Log(keyboard.InputField.text);
    }

}
