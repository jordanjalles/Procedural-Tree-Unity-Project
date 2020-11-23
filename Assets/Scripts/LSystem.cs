using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystem 
{
    //initial string that we start from
    public string initialString;

    //rewriting rules
    public Dictionary<string, string> iterationRules;

    public string currentString;

    public LSystem(string initialString, Dictionary<string, string> iterationRules)
    {
        this.initialString = initialString;
        currentString = initialString;
        this.iterationRules = iterationRules;
    }
    public string Iterate()
    {
        List<string> nextIteration = new List<string>(); 

        for (int i = 0; i < currentString.Length; i++)
        {
            if (iterationRules.ContainsKey(currentString[i].ToString())) { 
               nextIteration.Add(iterationRules[currentString[i].ToString()]);
            }
            else
            {
                nextIteration.Add(currentString[i].ToString());
            }

        }

        return string.Join("", nextIteration);
    }
}
