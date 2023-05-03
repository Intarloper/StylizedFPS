using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public Transform camPosition;
    public PlayerMovement playerMovement;
    // Shake Parameters
    public float shakeDuration = 2f;
    public float shakeAmount = 0.7f;

    //decimal values
    public float shakeIntensity;

    private float _shakeTimer = 0;
    

 


    public IEnumerator StartCameraShakeEffect()
    {
        Vector3 startingPosition = transform.localPosition;
        while (_shakeTimer < shakeDuration)
        {
            if(playerMovement.isSliding){
                transform.localPosition = new Vector3(
	            (Mathf.PerlinNoise(0, Time.time * shakeAmount) * 2 - 1) * shakeIntensity,
	            (Mathf.PerlinNoise(1, Time.time * shakeAmount) * 2 - 1) * shakeIntensity,
	            transform.localPosition.z
                );
            _shakeTimer += Time.deltaTime;

            yield return null;
            }

            else{
                transform.localPosition = startingPosition;
                yield break;
            }
            

            
        }
        _shakeTimer = 0;
        transform.localPosition = startingPosition;
        
        
      
    }

    

}
