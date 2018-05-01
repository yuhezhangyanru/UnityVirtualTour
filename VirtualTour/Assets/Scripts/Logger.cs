using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Logger
{
    public static void Log(string str)
    {
        Debug.Log("[date=" + System.DateTime.Now.ToShortTimeString() + ",mili=" + System.DateTime.Now.Millisecond + "]=" + str);
    }
}
