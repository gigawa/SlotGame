using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class InputMangaer : MonoBehaviour
    {
        public delegate void RepeatBet();
        public event RepeatBet repeatBet;
        public bool repeatBetEnabled;

        public delegate void AddCredits();
        public event AddCredits addCredits;
        public bool acceptCredits;

        public delegate void CashOut();
        public event CashOut cashOut;
        public bool cashOutEnabled;

        public bool canChangeBet;
        public delegate void ChangeBet();
        public event ChangeBet changeBet;

        public delegate void IncreaseBet();
        public event IncreaseBet increaseBet;

        public delegate void DecreaseBet();
        public event DecreaseBet decreaseBet;

        public delegate void MinBet();
        public event MinBet minBet;

        public delegate void MaxBet();
        public event MaxBet maxBet;

        public void Update()
        {
            if(Input.GetButtonDown("Repeat Bet") && repeatBetEnabled)
            {
                repeatBet();
            }

            if (Input.GetButtonDown("Add Credits") && acceptCredits)
            {
                addCredits();
            }

            if (Input.GetButtonDown("Increase Bet") && canChangeBet)
            {
                increaseBet();
                changeBet();
            }

            if (Input.GetButtonDown("Decrease Bet") && canChangeBet)
            {
                decreaseBet();
                changeBet();
            }
        }

        public void DisableInputs()
        {
            repeatBetEnabled = false;
            acceptCredits = false;
            cashOutEnabled = false;
            canChangeBet = false;
        }

        public void EnableInputs()
        {
            repeatBetEnabled = true;
            acceptCredits = true;
            cashOutEnabled = true;
            canChangeBet = true;
        }
    }
}
