using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [System.Serializable]

    public class Reel : MonoBehaviour
    {
        [SerializeField]
        public List<Symbol> symbols;

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

        public string[] symbolList;

        // Start is called before the first frame update
        void Start()
        {
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

        public void CreateSymbols ()
        {
            float yPosition = transform.position.y;
            for(int i = 0; i < symbolList.Length - 1; i++)
            {
                GameObject symbolPrefab;
                foreach(var prefab in stateMachine.SymbolList)
                {
                    Debug.Log(prefab.name);
                    if(prefab.name == symbolList[i])
                    {
                        symbolPrefab = prefab;
                        var newSymbol = Instantiate(symbolPrefab, new Vector3(transform.position.x, yPosition, transform.position.z), transform.rotation, transform);
                        symbols.Add(newSymbol.GetComponent<Symbol>());
                        yPosition += spaceBetweenSymbols;
                        break;
                    }
                }
            }

            yPosition = transform.position.y - 2;
            foreach (var prefab in stateMachine.SymbolList)
            {
                Debug.Log(prefab.name);
                if (prefab.name == symbolList[symbolList.Length - 1])
                {
                    var symbolPrefab = prefab;
                    var newSymbol = Instantiate(symbolPrefab, new Vector3(transform.position.x, yPosition, transform.position.z), transform.rotation, transform);
                    symbols.Add(newSymbol.GetComponent<Symbol>());
                    yPosition += spaceBetweenSymbols;
                    break;
                }
            }
        }

        /// <summary>
        /// Sets the new stop position for the reels
        /// </summary>
        /// <param name="newStop"></param>
        public void SetTargetStopPosition(int newStop)
        {
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
            int distanceToStart = targetStopPos - stopPosition > 0 ? targetStopPos - stopPosition : targetStopPos - stopPosition + symbols.Count;

            // Add rotation if only have 1/4 of the reel extra to reach stop position
            int distanceToTravel = distanceToStart > symbols.Count / 4 ? rotations * symbols.Count + distanceToStart : (rotations + 1) * symbols.Count + distanceToStart;
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
            int position = stopPosition - 1 > -1 ? stopPosition - 1 : symbols.Count - 1;
            for (int i = 0; i < 5; i++)
            {
                symbols[position].gameObject.transform.localPosition = new Vector3(symbols[position].gameObject.transform.localPosition.x, symbols[position].gameObject.transform.localPosition.y - spaceBetweenSymbols * Time.deltaTime * spinSpeed, symbols[position].gameObject.transform.position.z);
                position = position + 1 < symbols.Count ? position + 1 : 0;
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

            rawPosition = (rawPosition + Time.deltaTime * spinSpeed) % (symbols.Count);
            stopPosition = (int)rawPosition;
            currSpinTime += Time.deltaTime;
        }

        /// <summary>
        /// Finish spin and set symbol positions where they should be, 
        /// compensating for rounding and timing errors in stopping the reels
        /// </summary>
        void CompleteSpin()
        {
            //// get position of bottom symbol
            //int position = stopPosition - 2 > -1 ? stopPosition - 2 : ((symbols.Count - 1) - (1 - stopPosition));

            //// set y position for each symbol
            //float yPosition = -2 * spaceBetweenSymbols;

            //for (int i = 0; i < symbols.Count; i++)
            //{
            //    symbols[position].gameObject.transform.localPosition = new Vector3(symbols[position].gameObject.transform.localPosition.x, yPosition, symbols[position].gameObject.transform.position.z);

            //    // increment position on reels up one
            //    position = position + 1 < symbols.Count ? position + 1 : 0;

            //    yPosition += spaceBetweenSymbols;
            //}

            int position = stopPosition - 1 > -1 ? stopPosition - 1 : symbols.Count - 1;
            float yPosition = -2 * spaceBetweenSymbols;
            for (int i = 0; i < 4; i++)
            {
                symbols[position].gameObject.transform.localPosition = new Vector3(symbols[position].gameObject.transform.localPosition.x, yPosition, symbols[position].gameObject.transform.position.z);
                position = position + 1 < symbols.Count ? position + 1 : 0;
                yPosition += spaceBetweenSymbols;
            }
            symbols[position].gameObject.transform.localPosition = new Vector3(symbols[position].gameObject.transform.localPosition.x, topPosition, symbols[position].gameObject.transform.position.z);

            spinning = false;
            currSpinTime = 0f;

            stopReels(this);
        }

        public void SetStop(int stop)
        {
            stopPosition = stop;
            targetStopPos = stop;

            // get position of bottom symbol
            int position = stopPosition - 2 > -1 ? stopPosition - 2 : ((symbols.Count - 1) - (1 - stopPosition));

            // set y position for each symbol
            float yPosition = -2 * spaceBetweenSymbols;
            for (int i = 0; i < symbols.Count; i++)
            {
                symbols[position].gameObject.transform.localPosition = new Vector3(symbols[position].gameObject.transform.localPosition.x, yPosition, symbols[position].gameObject.transform.position.z);

                // increment position on reels up one
                position = position + 1 < symbols.Count ? position + 1 : 0;

                yPosition += spaceBetweenSymbols;
            }
        }
    }
}