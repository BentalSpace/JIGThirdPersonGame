using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static float gameTime;
    
    private void Awake() {
        gameTime = 0;
    }

    
    void Update()
    {
        gameTime += Time.deltaTime;
    }
}
