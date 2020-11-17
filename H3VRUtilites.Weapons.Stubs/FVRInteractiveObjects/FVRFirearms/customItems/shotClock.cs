﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using FistVR;

namespace H3VRUtilities.customItems.shotClock
{
	class shotClock : MonoBehaviour
	{
		public string stopclocktextstring;

		public Vector2 startingTimeWindow = new Vector2(2, 5);
		public float stopclock;
		public int shotsfired;

		public GameObject startbutton;
		public GameObject stopbutton;

		public bool isClockOn;
		public bool isInDelayProcess;

		public Text stopclocktext;
		public Text lastshottext;
		public Text shotsfiredtext;
		public Text delaymintext;
		public Text delaymaxtext;

		private float waittime;

		public FVRFireArmChamber[] registery;

		private bool[] registerySpent;

		public Text[] weptext;

		public BoxCollider registertrigger;

		public GameObject[] chambersInScene;

		private bool alreadyInRegisteryFlag;

		public enum screen
		{
			shot,
			register,
			delay
		}

		public screen currentScreen;

		public void Awake()
		{
			registerySpent = new bool[11];

		}

		public void startClockProcess()
		{
			waittime = UnityEngine.Random.Range(startingTimeWindow.x, startingTimeWindow.y);
			isInDelayProcess = true;
		}

		void Update()
		{
			if (isClockOn)
			{
				stopclock += Time.deltaTime;
				stopclocktext.text = updateStopClockTextString();
			}
			if (isInDelayProcess)
			{
				stopclock += Time.deltaTime;
				if (stopclock > waittime)
				{
					isInDelayProcess = false;
					StartClock();
				}
			}
			if (isClockOn || isInDelayProcess)
			{
				startbutton.SetActive(false);
				stopbutton.SetActive(true);
			}
			else
			{
				startbutton.SetActive(true);
				stopbutton.SetActive(false);
			}

			for (int i = 0; i < registery.Length; i++)
			{
				if (registery[i] != null)
				{
					if (registery[i].IsSpent)
					{
						if (!registerySpent[i])
						{
							registerySpent[i] = true;
							ShotDetected();
						}
					}
					else
					{
						registerySpent[i] = false;
					}
				}
			}

			if (currentScreen == screen.register)
			{
				updateRegistery();
			}
		}

		void updateRegistery()
		{
			//get all firearmchambers
			chambersInScene = GameObject.FindGameObjectsWithTag("FVRFireArmChamber");
			for (int i = 0; i < chambersInScene.Length; i++)
			{
				//for every chamber, check if it's inside registertrigger bounds
				if (registertrigger.bounds.Contains(chambersInScene[i].transform.position))
				{
					var cischamber = chambersInScene[i].GetComponent<FVRFireArmChamber>();
					//check if the chamber is already loaded into the registery
					for (int io = 0; io < registery.Length; io++)
					{
						if (registery[io] != null)
						{
							if (registery[io] == cischamber)
							{
								alreadyInRegisteryFlag = true;
							}
						}
					}
					//if it isn't, load it in
					if (!alreadyInRegisteryFlag)
					{
						for (int ip = 0; ip < registery.Length; ip++)
						{
							//get the first null in registery
							if (registery[i] == null)
							{
								registery[i] = chambersInScene[i].GetComponent<FVRFireArmChamber>();
								break;
							}
						}
						updateRegisteryText();
					}
				}
			}
		}

		public void updateRegisteryText()
		{
			for(int i = 0; i < registery.Length; i++)
			{
				if (registery[i] != null)
				{
					weptext[i].text = registery[i].transform.root.ToString();
				}
				else
				{
					weptext[i].text = "EMPTY";
				}
			}
		}

		public string updateStopClockTextString()
		{
			stopclocktextstring = "";
			var ts = TimeSpan.FromSeconds(stopclock);
			if (ts.Minutes < 10)
			{
				stopclocktextstring += "0";
			}
			stopclocktextstring += ts.Minutes.ToString() + ":";
			if (ts.Seconds < 10)
			{
				stopclocktextstring += "0";
			}
			stopclocktextstring += ts.Seconds.ToString() + ":";
			if (ts.Milliseconds < 100)
			{
				stopclocktextstring += "0";
				if (ts.Milliseconds < 10)
				{
					stopclocktextstring += "0";
				}
			}
			stopclocktextstring += Math.Round((double)ts.Milliseconds, 3).ToString();
			return stopclocktextstring;
		}

		public void StartClock()
		{
			isClockOn = true;
			stopclock = 0;
		}

		public void StopClock()
		{
			isClockOn = false;
			stopclock = 0;
		}

		public void ShotDetected()
		{
			if (isClockOn)
			{
				var ts = TimeSpan.FromSeconds(stopclock);
				lastshottext.text = updateStopClockTextString();
			}
		}
	}
}