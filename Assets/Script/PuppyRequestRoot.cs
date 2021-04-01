using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * This is a container class to handle our JSON result from the API 
 */


public class PuppyRequestRoot
{
    public string title { get; set; }
    public double version { get; set; }
    public string href { get; set; }
    public List<Result> results { get; set; }
}

public class Result
{
    public string title { get; set; }
    public string href { get; set; }
    public string ingredients { get; set; }
    public string thumbnail { get; set; }
}
