using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseInteraction : Interaction
{
    [SerializeField] protected Response[] Responses;
    public Response[] responses => Responses;

    public bool HasResponses() => responses != null && responses.Length > 0 && !AutoPickResponse();

    public bool AutoPickResponse() => responses != null && responses.Length == 1 && responses[0].text == string.Empty;
}

[Serializable]
public class Response
{
    [SerializeField] private string Text;
    [SerializeField] private Interaction Result;
    [SerializeField] private ResponseSFX SFX;
    public string text => Text;
    public Interaction result => Result;
    public ResponseSFX sFX => SFX;

    public Response(string text, Interaction result)
    {
        Text = text;
        Result = result;
    }
}

public enum ResponseSFX { None, Correct, Wrong }
