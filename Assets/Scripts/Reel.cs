using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [System.Serializable]

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

        public int targetStopPos;

        public int rotations;

        public float currSpinTime;
        public float desiredSpinSpeed;
        public float spinSpeed;
        public float topPosition;
        public int maxRng;

        private float spinTime;

        public float symbolDistance;

        public StateMachine stateMachine;

        public delegate void StartReels(Reel reel);
        public StartReels startReels;

        public delegate void StopReels(Reel reel);
        public StopReels stopReels;

        // Start is called before the first frame update
        void Start()
        {
            symbols = GetComponentsInChildren<Symbol>();
            maxRng = 0;
            foreach(Symbol symbol in symbols)
            {
                if(symbol.maxRng > maxRng)
                {
                    maxRng = symbol.maxRng;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (spinning && (stopPosition != targetStopPos || currSpinTime < (spinTime - 0.25f)))
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
        public void StartSpin(float time)
        {
            spinning = true;

            // Determine by desired speed
            //int distanceToStart = targetStopPos - stopPosition > 0 ? targetStopPos - stopPosition : targetStopPos - stopPosition + symbols.Length;
            //float firstRotationTime = distanceToStart * spaceBetweenSymbols / desiredSpinSpeed;
            //float rotationTime = symbols.Length * spaceBetweenSymbols / desiredSpinSpeed;
            //int desiredRotations = Mathf.RoundToInt(time - firstRotationTime > rotationTime ? (time - firstRotationTime) / rotationTime : 1);
            //int distanceToTravel = desiredRotations * symbols.Length + distanceToStart;
            //spinSpeed = distanceToTravel / time;
            //spinTime = time;

            // Determine by desired rotations
            int distanceToStart = targetStopPos - stopPosition > 0 ? targetStopPos - stopPosition : targetStopPos - stopPosition + symbols.Length;

            // Add rotation if only have 1/4 of the reel extra to reach stop position
            int distanceToTravel = distanceToStart > symbols.Length / 4 ? rotations * symbols.Length + distanceToStart : (rotations + 1) * symbols.Length + distanceToStart;
            spinSpeed = distanceToTravel / time;
            spinTime = time;

            ResetSymbolEffects();

            rawPosition = stopPosition;

            startReels(this);
        }

        public void ResetSymbolEffects()
        {
            foreach (Symbol symbol in symbols)
            {
                symbol.StopWin();
            }
        }

        /// <summary>
        /// Moves symbols to simulate spinning reel
        /// </summary>
        void SpinReel()
        {
            int position = stopPosition - 1 > -1 ? stopPosition - 1 : symbols.Length - 1;
            Debug.Log("Start Position: " + position);
            for (int i = 0; i < 4; i++)
            {
                symbols[position].gameObject.transform.localPosition = new Vector3(symbols[position].gameObject.transform.localPosition.x, symbols[position].gameObject.transform.localPosition.y - spaceBetweenSymbols * Time.deltaTime * spinSpeed, symbols[position].gameObject.transform.position.z);
                position = position + 1 < symbols.Length ? position + 1 : 0;
            }
            symbols[position].gameObject.transform.localPosition = new Vector3(symbols[position].gameObject.transform.localPosition.x, topPosition, symbols[position].gameObject.transform.position.z);

            //for (int i = 0; i < symbols.Length; i++)
            //{
            //    symbols[i].gameObject.transform.localPosition = new Vector3(symbols[i].gameObject.transform.localPosition.x, symbols[i].gameObject.transform.localPosition.y - spaceBetweenSymbols * Time.deltaTime * spinSpeed, symbols[i].gameObject.transform.position.z);

            //    // move symbols back to top of reel
            //    if(symbols[i].gameObject.transform.localPosition.y < (-1 * spaceBetweenSymbols * 2))
            //    {
            //        symbols[i].gameObject.transform.localPosition = new Vector3(symbols[i].gameObject.transform.localPosition.x, topPosition, symbols[i].gameObject.transform.position.z);
            //    }
            //}

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

            stopReels(this);
        }

        public void SetStop(int stop)
        {
            stopPosition = stop;
            targetStopPos = stop;

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
        }
    }
}