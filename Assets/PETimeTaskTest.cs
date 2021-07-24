using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FinTOKMAK.PETimeTask;

public class PETimeTaskTest : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        TimeSystem.Instance.AddTimeTask((id,dt,it,c) => {

            this.gameObject.SetActive(false);
        
        }, 2, PETimeUnit.Second, 1);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
