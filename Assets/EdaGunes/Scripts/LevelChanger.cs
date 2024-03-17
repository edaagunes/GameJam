using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
   public Animator animator;
   private int levelToLoad; 
   

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.CompareTag("Player"))
      {
         FadeToNextLevel();
      }
   }

   private void FadeToNextLevel()
   {
      FadeToLevel(SceneManager.GetActiveScene().buildIndex + 1);
   }
   
   public void FadeToLevel (int levelIndex)
   {
      levelToLoad = levelIndex;
      animator.SetTrigger("FadeOut");
   }

   public void OnFadeComplete ()
   {
      SceneManager.LoadScene(levelToLoad);
   }
}
