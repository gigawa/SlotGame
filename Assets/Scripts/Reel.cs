using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [System.Serializable]
    public class Symbol
    {
        public GameObject gameObject;
        public GameObject winEffect;
        public int minRng;
        public int maxRng;
    }

    public class Reel : MonoBehaviour
    {
        [SerializeField]
        public Symbol[] symbols;

        [SerializeField]
        public int stopPosition { get; private set; }
        private float rawPosition;

        [SerializeField]
        public bool spinning { get; private set; }

        [SerializeField]
        private float spaceBetweenSymbols;

        [SerializeField]
        private int targetStopPos;

        public float minSpinTime;
        public float currSpinTime;
        public float spinSpeed;
        public float topPosition;
        public int maxRng;

        public StateMachine stateMachine;

        // Start is called before the first frame update
        void Start()
        {
            maxRng = 0;
            foreach(Symbol symbol in symbols)
            {
                if(symbol.maxRng > maxRng)
                {
                    maxRng = symbol.maxRng;
                }

                symbol.winEffect = symbol.gameObject.transform.Find("WinEffect").gameObject;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (spinning && (stopPosition != targetStopPos || currSpinTime < minSpinTime))
            {
                SpinReel();
            }
            else if (spinning)
            {
                CompleteSpin();
            }
        }

        /// <summary>
        /// Sets the new stop position for the reels
        /// </summary>
        /// <param name="newStop"></param>
        public void SetTargetStopPosition(int rng)
        {
            int newStop = 0;
            //Debug.Log("Rng: " + rng);
            for (int i = 0; i < symbols.Length; i++)
            {
                if(symbols[i].minRng < rng && symbols[i].maxRng > rng)
                {
                    newStop = i;
                    break;
                }
            }
            targetStopPos = newStop;
        }

        /// <summary>
        /// Start spinning the reels
        /// </summary>
        public void StartSpin()
        {
            spinning = true;

            ResetSymbolEffects();
        }

        public void ResetSymbolEffects()
        {
            foreach (Symbol symbol in symbols)
            {
                symbol.winEffect.SetActive(false);
            }
        }

        /// <summary>
        /// Moves symbols to simulate spinning reel
        /// </summary>
        void SpinReel()
        {
            for(int i = 0; i < symbols.Length; i++)
            {
                symbols[i].gameObject.transform.localPosition = new Vector3(symbols[i].gameObject.transform.localPosition.x, symbols[i].gameObject.transform.localPosition.y - spaceBetweenSymbols * Time.deltaTime * spinSpeed, symbols[i].gameObject.transform.position.z);

                // move symbols back to top of reel
                if(symbols[i].gameObject.transform.localPosition.y < (-1 * spaceBetweenSymbols * 2))
                {
                    symbols[i].gameObject.transform.localPosition = new Vector3(symbols[i].gameObject.transform.localPosition.x, topPosition, symbols[i].gameObject.transform.position.z);
                }
            }

            rawPosition = (rawPosition + Time.deltaTime * spinSpeed) % (symbols.Length);
            stopPosition = (int)rawPosition;
            currSpinTime += Time.deltaTime;
        }

        /// <summary>
        /// Finish spin and set symbol positions where they should be, 
        /// compensating for rounding and timing errors in stopping the reels
        /// </summary>
        void CompleteSpin()
        {
            // get position of bottom symbol
            int position = stopPosition - 2 > -1 ? stopPosition - 2 : ((symbols.Length - 1) - (1 - stopPosition));

            // set y position for each symbol
            float yPosition = -2 * spaceBetweenSymbols;
            for (int i = 0; i < symbols.Length; i++)
            {
                symbols[position].gameObject.transform.localPosition = new Vector3(symbols[position].gameObject.transform.localPosition.x, yPosition, symbols[position].gameObject.transform.position.z);

                // increment position on reels up one
                position = position + 1 < symbols.Length ? position + 1 : 0;

                yPosition += spaceBetweenSymbols;
            }

            spinning = false;
            currSpinTime = 0f;
        }
    }
}