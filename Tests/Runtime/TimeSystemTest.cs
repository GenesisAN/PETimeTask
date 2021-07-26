using System.Collections;
using System.Collections.Generic;
using FinTOKMAK.PETimeTask;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TimeSystemTest
{
    private TimeSystem _timeSystem;
    
    /// <summary>
    /// Initialize TimeSystem
    /// </summary>
    [SetUp]
    public void Init()
    {
        _timeSystem = Object.Instantiate(new GameObject()).AddComponent<TimeSystem>();
    }

    /// <summary>
    /// TearDown code
    /// Remove the GameObject created by test
    /// </summary>
    [TearDown]
    public void End()
    {
        Object.DestroyImmediate(_timeSystem.gameObject);
    }
    
    // A Test behaves as an ordinary method
    [Test]
    public void TimeSystemSingletonTest()
    {
        // check the singleton creation
        Assert.IsTrue(TimeSystem.Instance == _timeSystem);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TimeSystemDelayExecute()
    {
        Debug.Log(Time.realtimeSinceStartup);
        
        // delay 3 seconds and debug log a text
        TimeSystem.Instance.AddTimeTask(((id, nextTime, interval, exeTime) =>
        {
            Debug.Log("Execute delay 3 seconds.");
            Debug.Log(Time.realtimeSinceStartup);
        }), 3, PETimeUnit.Second, 1);
        
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return new WaitForSeconds(5);
        
        Debug.Log("Test end.");
        Debug.Log(Time.realtimeSinceStartup);
    }

    [UnityTest]
    public IEnumerator TimeSystemPerformanceTest()
    {
        float iterateTimes = 100000;
        float startTime = Time.realtimeSinceStartup;
        Debug.Log("Start time: " + startTime);
        Debug.Log($"Start adding tasks {iterateTimes} times.");
        Debug.Log("==========");
        for (int i = 0; i < iterateTimes; i++)
        {
            TimeSystem.Instance.AddTimeTask(((id, nextExeTime, interval, exeTimes) =>
            {
                
            }), 10, PETimeUnit.Second, 1);
        }
        float endTime = Time.realtimeSinceStartup;
        Debug.Log($"Finish adding tasks {iterateTimes} times.");
        Debug.Log("End time: " + endTime);
        Debug.Log("Total execute time: " + (endTime - startTime));

        yield return null;
    }
}
