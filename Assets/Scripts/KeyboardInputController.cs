using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputController : MonoBehaviour {


    public string RedTeamRFID = "";
    public string GreenTeamRFID = "";

    private int _CurRedIndex = 0;
    private int _CurGreenIndex = 0;
    private string _UserString = "";

    void CheckKeyCode()
    {
        if (Input.anyKeyDown)
        {

            if(Input.GetKeyDown(RedTeamRFID[_CurRedIndex].ToString()))
            {
                _CurRedIndex++;
                if(_CurRedIndex >= RedTeamRFID.Length)
                {
                    TriggerWin("red");
                }
            }else {
                _CurRedIndex = 0;
            }

            if (Input.GetKeyDown(GreenTeamRFID[_CurGreenIndex].ToString()))
            {
                _CurGreenIndex++;
                if (_CurGreenIndex >= GreenTeamRFID.Length)
                {
                    TriggerWin("green");
                }
            }
            else
            {
                _CurGreenIndex = 0;
            }

        }
    }

    void TriggerWin(string teamID)
    {
        if(!string.IsNullOrEmpty(teamID))
        {
            Debug.Log("this team won: " + teamID);
            switch(teamID) {
                case "red":
                    RedTeamWin();
                    break;
                case "green":
                    GreenTeamWin();
                    break;
            }
            _CurRedIndex = 0;
            _CurGreenIndex = 0;
        }
    }

    // Update is called once per frame
    void Update () {
        CheckKeyCode();
	}

    public void RedTeamWin()
    {
        Debug.Log("red team keycode triggered");
        EventManager.Instance.RedTeamWin();
    }

    public void GreenTeamWin()
    {
        Debug.Log("green team keycode triggered");
        EventManager.Instance.GreenTeamWin();
    }
}
