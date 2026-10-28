using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Game
{
    public class TestMethod : MonoBehaviour
    {
        [SerializeField] private TMP_Text textCount;
        
        
        private int count;
        private float lastRealTimeCheck;
        
        
        private void Start()
        {
           
        }


        private void FixedUpdate()
        {
            var hit = Physics2D.Raycast(new Vector2(0, -2), Vector2.up, 1000);
            textCount.text = hit ? "True" : "False";
            
            // Debug.Log("Running");
            // count++;
            //
            // if(Time.realtimeSinceStartup - lastRealTimeCheck < 1)
            //     return;
            //
            // lastRealTimeCheck = Time.realtimeSinceStartup;
            // textCount.text = count.ToString();
            // count = 0;
        }
    }
}