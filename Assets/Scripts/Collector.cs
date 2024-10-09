using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collector : MonoBehaviour
{
    [SerializeField] Text countText;
    private int countPine=0;
    
    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag=="fruit"){
            countPine++;
            countText.text="PINE: "+ countPine;
            Destroy(other.gameObject);
        }
    }

    
}
