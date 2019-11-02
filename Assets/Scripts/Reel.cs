using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Reel : MonoBehaviour
    {

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

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log(symbols[7].transform.localPosition.y);
        }

        // Update is called once per frame
        void Update()
        {
            if (spinning && (stopPosition <= targetStopPos || currSpinTime < minSpinTime))
            {
                SpinReel();
            }
            else if (spinning)
            {
                spinning = false;
                currSpinTime = 0f;
            }
        }

        /// <summary>
        /// Sets the new stop position for the reels
        /// </summary>
        /// <param name="newStop"></param>
        public void SetTargetStopPosition(int newStop)
        {
            targetStopPos = newStop;
            spinning = true;
            //SpinReel();
        }

        void SpinReel()
        {
            for(int i = 0; i < symbols.Length; i++)
            {
                symbols[i].transform.localPosition = new Vector3(symbols[i].transform.localPosition.x, symbols[i].transform.localPosition.y - spaceBetweenSymbols * Time.deltaTime * spinSpeed, symbols[i].transform.position.z);

                if(symbols[i].transform.localPosition.y < (-1 * spaceBetweenSymbols * 2))
                {
                    GameObject topSymbol = (i - 1) >= 0 ? symbols[i-1] : symbols[symbols.Length - 1];
                    Debug.Log(topSymbol.name);
                    symbols[i].transform.localPosition = new Vector3(symbols[i].transform.localPosition.x, topSymbol.transform.localPosition.y + spaceBetweenSymbols, symbols[i].transform.position.z);
                }
            }

            rawPosition = (rawPosition + Time.deltaTime * spinSpeed) % (symbols.Length);
            stopPosition = (int)rawPosition;
            currSpinTime += Time.deltaTime;
        }

        void CompleteSpin()
        {

        }
    }
}