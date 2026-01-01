using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class GettingVersions
{
    public int id { get; set; }
    public string eschool_app_version { get; set; }
    public string alquran_app_version { get; set; }
    public object created_at { get; set; }
    public object updated_at { get; set; }
    public string eschool_app_huawei_version { get; set; }
    public string eschool_app_ios_version { get; set; }
    public string alquran_app_huawei_version { get; set; }
    public string alquran_app_ios_version { get; set; }
}
public class GetVersionFromApi
{
    public bool status { get; set; }
    public string message { get; set; }
    public GettingVersions data { get; set; }
}


