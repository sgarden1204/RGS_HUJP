using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UI;

public class PlayerBoardGUI : MonoBehaviour {

    private RaceController raceController;
    private CarGUI carGUI;

    public Image carIcon;

    public Sprite emptyCar;

    public Text userMessage;

    public bool show_Time = false;
	
	void Update () {
		if(raceController == null)
        {
            carIcon.sprite = emptyCar;
            userMessage.text = "";
            return;
        }

        carIcon.sprite = carGUI.GetIcon();
        string text = raceController.photonView.owner.NickName;

        if(show_Time)
        {
            text += " - " + FormatTime(raceController.raceTime);
        }
	}

    public string FormatTime(float time)
    {
        //Mathf.Ceiling 올림
        //Mathf.Round 반올림
        //Mathf.Floor 내림
        //Mathf.Truncate 버림
        string minutes = Mathf.Floor(time / 60).ToString("00");
        string seconds = Mathf.Floor(time % 60).ToString("00");
        string milis = Mathf.Floor((time * 1000) % 1000).ToString("000");
        return minutes + ":" + seconds + ":" + milis;
    }

    public void SetCar(RaceController raceController)
    {
        this.raceController = raceController;

        carGUI = raceController.GetComponent<CarGUI>();
    }
}
