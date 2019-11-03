using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Reel : MonoBehaviour
    {
        [SerializeField]
        public GameObject[] symbols;

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

        public StateMachine stateMachine;

        // Start is called before the first frame update
        void Start()
        {

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
        public void SetTargetStopPosition(int newStop)
        {
            targetStopPos = newStop;
        }

        public void StartSpin()
        {
            spinning = true;
        }

        void SpinReel()
        {
            for(int i = 0; i < symbols.Length; i++)
            {
                symbols[i].transform.localPosition = new Vector3(symbols[i].transform.localPosition.x, symbols[i].transform.localPosition.y - spaceBetweenSymbols * Time.deltaTime * spinSpeed, symbols[i].transform.position.z);

                if(symbols[i].transform.localPosition.y < (-1 * spaceBetweenSymbols * 2))
                {
                    symbols[i].transform.localPosition = new Vector3(symbols[i].transform.localPosition.x, topPosition, symbols[i].transform.position.z);
                }
            }

            rawPosition = (rawPosition + Time.deltaTime * spinSpeed) % (symbols.Length);
            stopPosition = (int)rawPosition;
            currSpinTime += Time.deltaTime;
        }

        void CompleteSpin()
        {
            int position = stopPosition - 2 > -1 ? stopPosition - 2 : ((symbols.Length - 1) - (1 - stopPosition));
            float yPosition = -2 * spaceBetweenSymbols;
            for (int i = 0; i < symbols.Length; i++)
            {
                symbols[position].transform.localPosition = new Vector3(symbols[position].transform.localPosition.x, yPosition, symbols[position].transform.position.z);

                position = position + 1 < symbols.Length ? position + 1 : 0;

                yPosition += spaceBetweenSymbols;
            }

            spinning = false;
            currSpinTime = 0f;
        }
    }
}