using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    Animator animator;
    

    private void Start(){
        animator= GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other){
        if(other.transform.tag=="trap"){
            //die
            animator.SetTrigger("death");
            GetComponent<PlayerMovement>().enabled=false;

             
        }
    }
    public void RestartLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
