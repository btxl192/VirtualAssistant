using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class config
{
    public static string domainName { get => "blueassistant-btxl.eastus.cloudapp.azure.com"; }
    public static ushort vmPort { get => 4430; }

    public static string alexaResponseIP { get => "172.17.0.1"; }
    public static ushort alexaResponsePort { get => 5000; }
}
